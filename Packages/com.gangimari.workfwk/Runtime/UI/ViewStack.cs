using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fwk.UI
{
    public class ViewStack
    {
        private readonly Canvas _canvas;
        private readonly Canvas _aboveBlurCanvas;
        private readonly Deque<StackableView> _stack = new();
        private Action<StackableView> _onNewFrontView;
        private readonly IBlurController _blurController;

        public ViewStack(string name, ViewStackSettings settings, Transform parent, IBlurController blurController)
        {
            _blurController = blurController;
            _canvas = new GameObject(name + "ViewStackCanvas").AddComponent<Canvas>();
            _canvas.transform.SetParent(parent, false);
            var canvasScaler = _canvas.gameObject.AddComponent<CanvasScaler>();
            _canvas.gameObject.AddComponent<GraphicRaycaster>();
            _canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            _canvas.sortingLayerName = settings.SortingLayerName;
            _canvas.renderMode = settings.RenderMode;
            _canvas.worldCamera = CameraManager.Instance.UICamera;
            _canvas.sortingOrder = settings.SortingOrder;
            _canvas.planeDistance = settings.PlaneDistance;
            _canvas.vertexColorAlwaysGammaSpace = settings.VertexColorAlwaysGammaSpace;
            canvasScaler.matchWidthOrHeight = settings.MatchWidthOrHeight;
            canvasScaler.uiScaleMode = settings.ScaleMode;
            canvasScaler.screenMatchMode = settings.ScreenMatchMode;
            canvasScaler.referenceResolution = settings.ReferenceResolution;

            _aboveBlurCanvas = new GameObject("AboveBlurCanvas").AddComponent<Canvas>();
            _aboveBlurCanvas.gameObject.AddComponent<GraphicRaycaster>();
            _aboveBlurCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
            _aboveBlurCanvas.transform.SetParent(_canvas.transform, false);
            _aboveBlurCanvas.overrideSorting = true;
            _aboveBlurCanvas.sortingLayerName = settings.AboveBlurSortingLayerName;
            _aboveBlurCanvas.sortingOrder = settings.SortingOrder;

            RectTransform rectTransform = _aboveBlurCanvas.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public void UpdateViewLoop(float deltaTime)
        {
            foreach (var view in _stack)
            {
                view.UpdateViewLoop(deltaTime);
            }
        }

        public void SetOnNewFrontView(Action<StackableView> onNewFrontView)
        {
            _onNewFrontView = onNewFrontView;
        }

        public void AddToFront(StackableView view)
        {
            if (_stack.Contains(view))
            {
                Debug.Log($"View {view} is already in the stack.");
                return;
            }

            if (_stack.Count > 0)
            {
                var lastView = _stack.PeekFront();
                lastView.OnCovered(view);
                PutViewBelowBlur(lastView);
            }

            _stack.AddToFront(view);
            Debug.Log($"Added {view} to front of stack.");

            OnNewFrontView(view);
        }

        public void AddToBack(StackableView view)
        {
            if (_stack.Contains(view))
            {
                Debug.Log($"View {view} is already in the stack.");
                return;
            }
            _stack.AddToBack(view);
            Debug.Log($"Added {view} to back of stack.");

            if (_stack.Count == 1)
            {
                OnNewFrontView(view);
            }
            else
            {
                PutViewBelowBlur(view);
            }
        }

        public void RemoveFromFront(StackableView view)
        {
            if (_stack.PeekFront() != view)
            {
                Debug.LogError($"{view} is not at the front of the stack.");
                return;
            }

            RemoveFromFront();
        }

        public void RemoveFromBack(StackableView view)
        {
            if (_stack.PeekBack() != view)
            {
                Debug.LogError($"{view} is not at the back of the stack.");
                return;
            }

            RemoveFromBack();
        }

        public void RemoveFromFront()
        {
            var stackView = _stack.RemoveFromFront();
            Debug.Log($"Removed {stackView} from front of stack.");
            OnFrontViewRemoved(stackView);

            if (_stack.Count > 0)
            {
                var nextView = _stack.PeekFront();
                OnNewFrontView(nextView);
            }
        }

        public void RemoveFromBack()
        {
            bool isFrontView = _stack.Count == 1;
            var stackView = _stack.RemoveFromBack();
            Debug.Log($"Removed {stackView} from back of stack.");

            if (isFrontView)
            {
                OnFrontViewRemoved(stackView);
            }
            else
            {
                stackView.OnRemoveFromBack();
            }
        }

        private void OnNewFrontView(StackableView view)
        {
            if (view.BlurBackground)
            {
                _blurController.Blur();
            }
            PutViewAboveBlur(view);
            _onNewFrontView?.Invoke(view);
            view.OnFront();
        }

        private void OnFrontViewRemoved(StackableView view)
        {
            if (view.BlurBackground)
            {
                _blurController.UnBlur();
            }
            PutViewBelowBlur(view);
            view.OnRemoveFromFront();
        }

        public bool Contains(StackableView view)
        {
            return _stack.Contains(view);
        }

        public StackableView PeekFront()
        {
            return _stack.PeekFront();
        }

        public StackableView PeekBack()
        {
            return _stack.PeekBack();
        }

        public void SetFrontViewBelowBlur()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            var frontView = _stack.PeekFront();
            PutViewBelowBlur(frontView);
        }

        public void SetFrontViewAboveBlur()
        {
            if (_stack.Count == 0)
            {
                return;
            }

            var frontView = _stack.PeekFront();
            PutViewAboveBlur(frontView);
        }

        private void PutViewAboveBlur(StackableView view)
        {
            view.IsAboveBlur = true;
            view.transform.SetParent(_aboveBlurCanvas.transform, false);
        }

        private void PutViewBelowBlur(StackableView view)
        {
            view.IsAboveBlur = false;
            view.transform.SetParent(_canvas.transform, false);
        }
    }
}