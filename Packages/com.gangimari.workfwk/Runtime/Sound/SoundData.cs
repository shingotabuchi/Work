using System;
using UnityEngine;

namespace Fwk.Sound
{
    [Serializable]
    public class SoundData : ISoundData
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private float _volume = 1.0f;

        public string Name => _clips != null && _clips.Length > 0 ? _clips[0].name : "Empty";

        public AudioClip Clip
        {
            get
            {
                if (_clips == null || _clips.Length == 0)
                    return null;

                if (_clips.Length == 1)
                    return _clips[0];

                // Return a random clip from the array
                int randomIndex = UnityEngine.Random.Range(0, _clips.Length);
                return _clips[randomIndex];
            }
        }

        public float Volume => _volume;
        public float PlayedVolume { get; set; } = 1.0f;
    }
}