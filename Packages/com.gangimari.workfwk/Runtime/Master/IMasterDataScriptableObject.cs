using System;
using System.Collections.Generic;

namespace Fwk.Master
{
    public interface IMasterDataScriptableObject
    {
        Type Type { get; }
        IReadOnlyList<IMasterData> Data { get; }
    }
}