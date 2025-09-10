using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fwk.Master
{
    public class MasterAsset<T> : ScriptableObject, IMasterDataScriptableObject where T : IMasterData
    {
        [SerializeField] private List<T> _data = new();
        public Type Type => typeof(T);
        public IReadOnlyList<IMasterData> Data => _data as IReadOnlyList<IMasterData>;
    }
}