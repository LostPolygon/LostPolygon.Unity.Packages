using System;
using System.Collections.Generic;

namespace LostPolygon.Unity.SimpleSoundSystem {
    public interface IAudioClipRepository<TSoundType> where TSoundType : Enum {
        IAudioMetaClipData GetByType(TSoundType type);
        IReadOnlyCollection<TSoundType> GetAllSoundTypes();
    }
}
