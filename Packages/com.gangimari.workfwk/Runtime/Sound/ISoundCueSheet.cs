using System.Collections.Generic;

namespace Fwk.Sound
{
    public interface ISoundCueSheet
    {
        string Name { get; }
        IReadOnlyList<ISoundData> SoundDatas { get; }
    }
}