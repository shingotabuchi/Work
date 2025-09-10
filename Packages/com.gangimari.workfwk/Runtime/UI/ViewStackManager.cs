using UnityEngine;
using Cysharp.Threading.Tasks;
using Fwk.Addressables;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Fwk.UI
{
    public class ViewStackManager : MonoBehaviour
    {
        private const string _defaultStackName = "Default";
        private const string _defaultAssetLabel = "StackableViews";
        private IAssetRequester _assetRequester;
        private bool _isInitialized = false;
        private bool _isInitializing = false;
        private readonly Dictionary<Type, StackableView> _uiCache = new();
        private readonly Dictionary<Type, GameObject> _uiPrefabCache = new();
        private readonly Dictionary<Type, Queue<StackableView>> _poolCache = new();
        private readonly Dictionary<string, ViewStack> _stackDict = new();
        private IBlurController _blurController;

        public async UniTask Initialize(
            IAssetRequester assetRequester,
            string assetLabel,
            ViewStackSettings defaultStackSettings,
            IBlurController blurController,
            CancellationToken token)
        {
            while (true)
            {
                if (_isInitialized)
                {
                    return;
                }
                if (_isInitializing)
                {
                    await UniTask.Yield(token);
                    continue;
                }
                _isInitializing = true;
                try
                {
                    _assetRequester = assetRequester;
                    _blurController = blurController;
                    await InitializeInternal(defaultStackSettings, token, assetLabel);
                }
                finally
                {
                    _isInitializing = false;
                }
                break;
            }
        }

        public void UpdateViewLoop(float deltaTime)
        {
            foreach (var stack in _stackDict.Values)
            {
                stack.UpdateViewLoop(deltaTime);
            }
        }

        private async UniTask InitializeInternal(
            ViewStackSettings defaultStackSettings,
            CancellationToken token,
            string assetLabel = _defaultAssetLabel)
        {
            if (CameraManager.Instance == null)
            {
                CameraManager.CreateIfNotExists();
            }

            var keys = await AddressableManager.GetKeysByLabel(assetLabel, cancellationToken: token);
            foreach (var key in keys)
            {
                Debug.Log($"Preloding stackable view: {key}");
            }
            await _assetRequester.Preload<GameObject>(keys, token);

            foreach (var key in keys)
            {
                var uiAsset = _assetRequester.GetAssetImmediate<GameObject>(key);
                var ui = Instantiate(uiAsset, transform);
                ui.SetActive(false);
                var view = ui.GetComponent<StackableView>();
                if (view == null)
                {
                    Debug.LogError($"View component not found in {key}");
                    continue;
                }
                var type = view.GetType();
                _uiCache.Add(type, view);
                _uiPrefabCache.Add(type, uiAsset);
            }

            CreateStack(_defaultStackName, defaultStackSettings);
            _isInitialized = true;
        }

        public void SetOnNewFrontView(Action<StackableView> onNewFrontView, string stackName = _defaultStackName)
        {
            if (!_stackDict.ContainsKey(stackName))
            {
                Debug.LogError($"Stack {stackName} not found.");
                return;
            }
            _stackDict[stackName].SetOnNewFrontView(onNewFrontView);
        }

        public void CreateStack(string stackName, ViewStackSettings settings)
        {
            if (_stackDict.ContainsKey(stackName))
            {
                Debug.Log($"Stack {stackName} already exists.");
                return;
            }
            var stack = new ViewStack(stackName, settings, transform, _blurController);
            _stackDict.Add(stackName, stack);
        }

        public T AddToFront<T>(string stackName = _defaultStackName) where T : StackableView
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return null;
            }
            
            var view = GetOrCreateView<T>();
            if (view == null)
            {
                Debug.LogError($"View not found for {typeof(T)}");
                return null;
            }
            var stack = _stackDict[stackName];
            stack.AddToFront(view);
            return view as T;
        }

        public T AddToBack<T>(string stackName = _defaultStackName) where T : StackableView
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return null;
            }
            
            var view = GetOrCreateView<T>();
            if (view == null)
            {
                Debug.LogError($"View not found for {typeof(T)}");
                return null;
            }
            var stack = _stackDict[stackName];
            stack.AddToBack(view);
            return view as T;
        }

        public void RemoveFromFront<T>(string stackName = _defaultStackName) where T : StackableView
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return;
            }
            
            var stack = _stackDict[stackName];
            var frontView = stack.PeekFront();
            
            if (frontView == null)
            {
                Debug.LogError($"No view at front of stack {stackName}");
                return;
            }
            
            if (!(frontView is T))
            {
                Debug.LogError($"Front view is not of type {typeof(T)}, it is {frontView.GetType()}");
                return;
            }
            
            stack.RemoveFromFront(frontView);
            
            // For multi-instance views, return to pool instead of destroying
            if (frontView is IMultiInstanceView)
            {
                ReturnToPool(frontView);
            }
        }

        public void RemoveFromBack<T>(string stackName = _defaultStackName) where T : StackableView
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return;
            }
            
            var stack = _stackDict[stackName];
            var backView = stack.PeekBack();
            
            if (backView == null)
            {
                Debug.LogError($"No view at back of stack {stackName}");
                return;
            }
            
            if (!(backView is T))
            {
                Debug.LogError($"Back view is not of type {typeof(T)}, it is {backView.GetType()}");
                return;
            }
            
            stack.RemoveFromBack(backView);
            
            // For multi-instance views, return to pool instead of destroying
            if (backView is IMultiInstanceView)
            {
                ReturnToPool(backView);
            }
        }

        public T GetView<T>() where T : StackableView
        {
            return _uiCache[typeof(T)] as T;
        }
        
        private T GetOrCreateView<T>() where T : StackableView
        {
            var type = typeof(T);
            
            // For multi-instance views, use object pool
            if (typeof(IMultiInstanceView).IsAssignableFrom(type))
            {
                return GetFromPool<T>();
            }
            
            // For single-instance views, use cached instance and check if already in stack
            var cachedView = _uiCache[type] as T;
            if (cachedView == null)
            {
                return null;
            }
            
            // Check if view is already in any stack
            foreach (var stack in _stackDict.Values)
            {
                if (stack.Contains(cachedView))
                {
                    Debug.Log($"View {cachedView} is already in a stack.");
                    return cachedView;
                }
            }
            
            return cachedView;
        }
        
        private T GetFromPool<T>() where T : StackableView
        {
            var type = typeof(T);
            
            if (!_poolCache.ContainsKey(type))
            {
                _poolCache[type] = new Queue<StackableView>();
            }
            
            var pool = _poolCache[type];
            
            // Try to get from pool first
            if (pool.Count > 0)
            {
                var pooledView = pool.Dequeue() as T;
                if (pooledView != null)
                {
                    pooledView.gameObject.SetActive(false);
                    if (pooledView is IMultiInstanceView multiInstanceView)
                    {
                        multiInstanceView.OnTakeFromPool();
                    }
                    return pooledView;
                }
            }
            
            // Create new instance if pool is empty
            if (!_uiPrefabCache.ContainsKey(type))
            {
                Debug.LogError($"Prefab not found for multi-instance view {type}");
                return null;
            }
            
            var prefab = _uiPrefabCache[type];
            var newInstance = Instantiate(prefab, transform);
            newInstance.SetActive(false);
            var view = newInstance.GetComponent<T>();
            if (view == null)
            {
                Debug.LogError($"View component not found in instantiated prefab for {type}");
                Destroy(newInstance);
                return null;
            }
            
            if (view is IMultiInstanceView multiInstanceViewNew)
            {
                multiInstanceViewNew.OnTakeFromPool();
            }
            
            return view;
        }
        
        private void ReturnToPool(StackableView view)
        {
            if (!(view is IMultiInstanceView multiInstanceView))
            {
                return;
            }
            
            var type = view.GetType();
            if (!_poolCache.ContainsKey(type))
            {
                _poolCache[type] = new Queue<StackableView>();
            }
            
            multiInstanceView.OnReturnToPool();
            view.gameObject.SetActive(false);
            _poolCache[type].Enqueue(view);
        }

        public void SetFrontViewBelowBlur(string stackName = _defaultStackName)
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return;
            }
            var stack = _stackDict[stackName];
            stack.SetFrontViewBelowBlur();
        }

        public void SetFrontViewAboveBlur(string stackName = _defaultStackName)
        {
            if (!_isInitialized)
            {
                Debug.LogError("ViewStackManager is not initialized.");
                return;
            }
            var stack = _stackDict[stackName];
            stack.SetFrontViewAboveBlur();
        }
    }
}