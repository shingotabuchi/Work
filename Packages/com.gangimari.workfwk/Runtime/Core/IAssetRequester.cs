using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;

namespace Fwk
{
    public interface IAssetRequester
    {
        UniTask<T> RequestAsset<T>(
            string key,
            CancellationToken cancellationToken = default
        ) where T : UnityEngine.Object;

        T GetAssetImmediate<T>(string key) where T : UnityEngine.Object;

        UniTask Preload<T>(
            IEnumerable<string> keys,
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null
        ) where T : UnityEngine.Object;

        bool IsPreloaded(IEnumerable<string> keys);
    }
}