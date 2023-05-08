using DG.Tweening;
using UnityEngine;

namespace LostPolygon.Unity.SimpleSoundSystem {
    /// <summary>
    /// Represents a currently playing sound.
    /// </summary>
    public sealed class PlayedSoundReference {
        private AudioSource _audioSource;
        private Tween _despawnTween;

        public AudioSource AudioSource => _audioSource;

        internal void Initialize(AudioSource audioSource, Tween despawnTween) {
            _audioSource = audioSource;
            _despawnTween = despawnTween;
        }

        public void Stop() {
            var despawnTween = _despawnTween;
            if (despawnTween == null)
                return;

            _despawnTween = null;
            if (despawnTween.active) {
                despawnTween.Kill(true);
            }
        }
    }
}
