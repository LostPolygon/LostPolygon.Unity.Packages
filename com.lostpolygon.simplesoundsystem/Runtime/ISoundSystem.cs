using System;

namespace LostPolygon.Unity.SimpleSoundSystem {
    public interface ISoundSystem<in TSoundType> where TSoundType : Enum {
        PlayedSoundReference PlaySound(TSoundType type, float delay = 0f);

        void StopAllSoundsOfType(TSoundType type);
    }
}
