using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Fwk.Addressables
{
    public class AddressableCache : IDisposable, IAssetRequester
    {
        private Dictionary<string, IAddressableHandle> _handles = new();
        private Dictionary<string, UniTask> _loadingTasks = new();
        private CancellationTokenSource _disposeCts = new();
        private bool _isDisposed = false;

        public async UniTask<T> RequestAsset<T>(
            string key,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object
        {
            return await LoadAsync<T>(key, cancellationToken);
        }

        public T GetAssetImmediate<T>(string key) where T : UnityEngine.Object
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AddressableCache));
            }

            if (TryGetHandle(key, out var handle))
            {
                return handle.Object as T;
            }
            else
            {
                Debug.LogWarning($"No handle found for key '{key}'. Returning null.");
                return null;
            }
        }

        public async UniTask Preload<T>(
            IEnumerable<string> keys,
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null
        ) where T : UnityEngine.Object
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AddressableCache));
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);
            var tasks = new List<UniTask>();
            foreach (var key in keys)
            {
                if (TryGetHandle(key, out var handle))
                {
                    continue;
                }

                tasks.Add(LoadAsync<T>(key, linkedCts.Token));
            }

            await UniTask.WhenAll(tasks);
        }

        public bool IsPreloaded(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (!TryGetHandle(key, out var handle))
                {
                    return false;
                }
            }
            return true;
        }

        public async UniTask<T> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null
        ) where T : UnityEngine.Object
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(AddressableCache));
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposeCts.Token);

            while (true)
            {
                if (TryGetHandle(key, out var handle))
                {
                    return handle.Object as T;
                }

                if (_loadingTasks.TryGetValue(key, out var loadingTask))
                {
                    try
                    {
                        await loadingTask;
                    }
                    catch
                    {
                        // continue if existing loading task failed
                    }
                    await UniTask.Yield();
                    continue;
                }

                try
                {
                    var task = LoadAsyncInternal();
                    _loadingTasks[key] = task;
                    await task;
                }
                catch (OperationCanceledException)
                {
                    TryReleaseHandle(key);
                    throw;
                }
                finally
                {
                    _loadingTasks.Remove(key);
                }

                if (TryGetHandle(key, out var loadedHandle))
                {
                    return loadedHandle.Object as T;
                }
                else
                {
                    Debug.LogError($"Failed to load asset of key '{key}' after loading task completed.");
                    throw new Exception($"Failed to load asset of key '{key}'.");
                }
            }

            async UniTask LoadAsyncInternal()
            {
                try
                {
                    var newHandle = await AddressableManager.LoadAsync<T>(key, progress, linkedCts.Token);

                    if (!newHandle.Succeeded)
                    {
                        Debug.LogError($"Failed to load asset of key '{key}'.");
                        throw new Exception($"Failed to load asset of key '{key}'.");
                    }

                    if (TryGetHandle(key, out var _))
                    {
                        Debug.LogWarning($"Key '{key}' already exists in the cache. Releasing the new handle.");
                        newHandle.Release();
                    }
                    else
                    {
                        _handles[key] = newHandle;
                    }
                }
                catch (OperationCanceledException)
                {
                    TryReleaseHandle(key);
                    throw;
                }
                catch (Exception ex)
                {
                    TryReleaseHandle(key);
                    Debug.LogError($"Failed to load asset of key '{key}': {ex}");
                    throw;
                }
            }
        }

        private bool TryGetHandle(string key, out IAddressableHandle handle)
        {
            if (_handles.TryGetValue(key, out var boxedHandle))
            {
                handle = boxedHandle;
                return true;
            }
            handle = null;
            return false;
        }

        private bool TryReleaseHandle(string key)
        {
            if (TryGetHandle(key, out var handle))
            {
                handle.Release();
                _handles.Remove(key);
                return true;
            }
            else
            {
                Debug.LogWarning($"No handle found for key '{key}'. Cannot release.");
            }
            return false;
        }

        public void Release(string key)
        {
            if (!TryReleaseHandle(key))
            {
                Debug.LogWarning($"No handle found for key '{key}'.");
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                handle.Release();
            }
            _handles.Clear();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            _disposeCts?.Cancel();
            _disposeCts?.Dispose();
            _disposeCts = null;

            ReleaseAll();
            _handles = null;

            _loadingTasks.Clear();
            _loadingTasks = null;
        }
    }
}
