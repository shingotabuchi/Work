using System;
using UnityEngine;

namespace Fwk.Master
{
    public class MasterData<T> : IMasterData
    {
        [SerializeField] private string _id;
        public string Id => _id;
        public Type Type => typeof(T);
    }
}