using System;
using System.Collections.Generic;
using LostPolygon.Unity.Utility;
using UnityEngine;

namespace LostPolygon.Unity.SimpleSoundSystem {
    [CreateAssetMenu]
    [Serializable]
    public abstract class AudioClipDataContainer<TSoundType, TAudioMetaClipData> :
        ScriptableObject,
        IAudioClipRepository<TSoundType>
        where TSoundType : Enum
        where TAudioMetaClipData : IAudioMetaClipData {
        [SerializeField]
        private SoundTypeToAudioMetaClipDataDictionary _soundTypeToMetaClip;

        public IAudioMetaClipData GetByType(TSoundType type) {
            if (_soundTypeToMetaClip.TryGetValue(type, out var metaClip))
                return metaClip;

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        public IReadOnlyCollection<TSoundType> GetAllSoundTypes() {
            return _soundTypeToMetaClip.Keys;
        }

        private void Reset() {
            _soundTypeToMetaClip.Clear();
        }

        [Serializable]
        private class SoundTypeToAudioMetaClipDataDictionary : UnitySerializedDictionary<TSoundType, TAudioMetaClipData> {
        }
    }
}
