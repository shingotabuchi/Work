using UnityEngine;
using System.Collections.Generic;

namespace Fwk.Sound
{
    [CreateAssetMenu(fileName = "SoundCueSheet", menuName = "SoundCueSheet", order = 0)]
    public class SoundCueSheet : ScriptableObject, ISoundCueSheet
    {
        [SerializeField] private List<SoundData> _soundDatas;
        public string Name => name;
        public IReadOnlyList<ISoundData> SoundDatas => _soundDatas;
    }
}