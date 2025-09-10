using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Fwk.Addressables
{
    public static class AddressableManager
    {
        public static async UniTask<AddressableHandle<T>> LoadAsync<T>(
            string key,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
            where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }

            AsyncOperationHandle<T> handle = new();
            try
            {
                handle = UnityAddressables.LoadAssetAsync<T>(key);
                while (!handle.IsDone)
                {
                    progress?.Report(handle.PercentComplete);
                    await UniTask.Yield(cancellationToken);
                }

                progress?.Report(1f);
                return new AddressableHandle<T>(handle);
            }
            catch (OperationCanceledException)
            {
                if (handle.IsValid())
                {
                    UnityAddressables.Release(handle);
                }
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load addressable asset with key '{key}': {ex}");
                if (handle.IsValid())
                {
                    UnityAddressables.Release(handle);
                }
                throw;
            }
        }

        public static async UniTask<AddressableHandle<IList<T>>> LoadByLabelAsync<T>(
            string label,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Label cannot be null or empty.", nameof(label));

            AsyncOperationHandle<IList<T>> handle = default;
            try
            {
                // callback is no-op; MergeMode.Union just means "all assets under this label"
                handle = UnityAddressables.LoadAssetsAsync<T>(
                    label,
                    null
                );

                while (!handle.IsDone)
                {
                    progress?.Report(handle.PercentComplete);
                    await UniTask.Yield(cancellationToken);
                }

                progress?.Report(1f);
                return new AddressableHandle<IList<T>>(handle);
            }
            catch (OperationCanceledException)
            {
                if (handle.IsValid())
                    UnityAddressables.Release(handle);
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load addressables with label '{label}': {ex}");
                if (handle.IsValid())
                    UnityAddressables.Release(handle);
                throw;
            }
        }

        public static async UniTask<IList<string>> GetKeysByLabel(string label, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            var handle = UnityAddressables.LoadResourceLocationsAsync(label);
            while (!handle.IsDone)
            {
                progress?.Report(handle.PercentComplete);
                await UniTask.Yield(cancellationToken);
            }
            progress?.Report(1f);

            var keys = new List<string>();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var locations = handle.Result;
                foreach (var location in locations)
                {
                    keys.Add(location.PrimaryKey);
                }
            }
            else
            {
                throw new Exception($"Failed to load resource locations for label '{label}'");
            }

            UnityAddressables.Release(handle);
            return keys;
        }
    }
}
