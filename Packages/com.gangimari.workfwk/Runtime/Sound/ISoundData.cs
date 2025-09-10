using UnityEngine;

namespace Fwk.Sound
{
    public interface ISoundData
    {
        string Name { get; }
        AudioClip Clip { get; }
        float Volume { get; }
        float PlayedVolume { get; set; }
    }
}