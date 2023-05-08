using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace LostPolygon.Unity.SimpleSoundSystem {
    /// <summary>
    /// A collection of audio clips for a game effect type.
    /// An audio clip will be randomly picked.
    /// </summary>
    [Serializable]
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class AudioMetaClipData : IAudioMetaClipData {
        [SerializeField]
#if ODIN_INSPECTOR
        [InlineProperty, ListDrawerSettings(Expanded = true, ShowItemCount = true)]
#endif
        private AudioClipData[] _clips = new AudioClipData[0];

        public AudioClipData[] Clips => _clips;
    }
}
