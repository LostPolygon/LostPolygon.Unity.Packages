using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace LostPolygon.Unity.SimpleSoundSystem {
    /// <summary>
    /// Represents a single audio clip and how it should be played.
    /// </summary>
    [Serializable]
#if ODIN_INSPECTOR
    [InlineProperty]
    [HideReferenceObjectPicker]
#endif
    public class AudioClipData {
        [SerializeField]
        private AudioClip _audioClip;

        [SerializeField]
        [Range(0f, 1f)]
        private float _volume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _pitchVariationDelta;

        [SerializeField]
        [Range(0f, 5f)]
        private float _fadeInDuration = 0;

        [SerializeField]
        [Range(0f, 5f)]
        private float _fadeOutDuration = 0;

        [SerializeField]
        private bool _loop = false;

        public AudioClip AudioClip => _audioClip;

        public float Volume => _volume;

        /// <summary>
        /// Determines the maximum pitch multiplier offset from the base value of 1.
        /// For example, a value of 0.1 would mean that the sound clip pitch will be randomly chosen
        /// in the [0.9; 1.1] range when the sound is played.
        /// </summary>
        public float PitchVariationDelta => _pitchVariationDelta;

        public float FadeInDuration => _fadeInDuration;

        public float FadeOutDuration => _fadeOutDuration;

        public bool Loop => _loop;
    }
}
