using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LostPolygon.Unity.SimpleSoundSystem {
    /// <summary>
    /// A very basic sound system with support for <see cref="AudioSource"/> pooling,
    /// per-clip volume and pitch variation, and per-effect clip randomization.
    /// </summary>
    public abstract class SoundSystem<TSoundType> : ISoundSystem<TSoundType> where TSoundType : Enum {
        private readonly IAudioClipRepository<TSoundType> _clipRepository;
        private readonly LeanGameObjectPool _audioSourcePool;
        private readonly Dictionary<TSoundType, HashSet<PlayedSoundReference>> _activeSounds = new();

        protected SoundSystem(
            IAudioClipRepository<TSoundType> clipRepository,
            LeanGameObjectPool audioSourcePool
        ) {
            _clipRepository = clipRepository ?? throw new ArgumentNullException(nameof(clipRepository));
            _audioSourcePool = audioSourcePool ? audioSourcePool : throw new ArgumentNullException(nameof(audioSourcePool));
        }

        public PlayedSoundReference PlaySound(TSoundType type, float delay = 0f) {
            var metaClipData = _clipRepository.GetByType(type);
            if (metaClipData.Clips.Length == 0)
                return null;

            var clipData = metaClipData.Clips[Random.Range(0, metaClipData.Clips.Length)];
            if (clipData.AudioClip == null) {
                Debug.LogWarning($"Audio definition has no clip {type}");
                return null;
            }

            var audioSourceGameObject = _audioSourcePool.Spawn(Vector3.zero, Quaternion.identity, _audioSourcePool.transform);
            if (audioSourceGameObject == null) {
                Debug.LogWarning($"Audio source pool depleted, sound {type} not played");
                return null;
            }

#if UNITY_EDITOR
            audioSourceGameObject.name = $"AudioSource [{type}]";
#endif

            var audioSource = audioSourceGameObject.GetComponent<AudioSource>();
            audioSource.clip = clipData.AudioClip;

            var logarithmicVolume = Mathf.Pow(clipData.Volume, (float) Math.E);
            audioSource.volume = Mathf.Abs(clipData.FadeInDuration) > Vector3.kEpsilon ? 0 : logarithmicVolume;

            var pitch = 1f;
            if (Mathf.Abs(clipData.PitchVariationDelta) > Vector3.kEpsilon) {
                pitch += Random.Range(-clipData.PitchVariationDelta, clipData.PitchVariationDelta);
            }

            audioSource.pitch = pitch;
            audioSource.loop = clipData.Loop;

            if (delay != 0f) {
                audioSource.PlayDelayed(delay);
            } else {
                audioSource.Play();
            }

            var audioClipLength = clipData.AudioClip.length / Mathf.Abs(pitch);
            audioClipLength += delay;

            // FIXME: is it necessary?
            // to be extra sure LeanPool won't cut the sound short
            audioClipLength += 0.1f;

            var playedSoundReference = LeanClassPool<PlayedSoundReference>.Spawn() ?? new PlayedSoundReference();
            var despawnSequence =
                DOTween.Sequence()
                    .Insert(
                        0,
                        audioSource
                            .DOFade(logarithmicVolume, clipData.FadeInDuration)
                            .SetEase(Ease.Linear)
                    );

            if (!clipData.Loop) {
                despawnSequence.InsertCallback(audioClipLength, () => {
                    // Just to ensure the tween length
                });
            } else {
                despawnSequence.SetAutoKill(false);
            }

            despawnSequence.OnKill(() => {
                if (Mathf.Abs(clipData.FadeOutDuration) > Vector3.kEpsilon) {
                    audioSource
                        .DOFade(0f, clipData.FadeOutDuration)
                        .SetEase(Ease.Linear)
                        .OnKill(() => KillSoundImmediately(type, audioSource, audioSourceGameObject, playedSoundReference));
                } else {
                    KillSoundImmediately(type, audioSource, audioSourceGameObject, playedSoundReference);
                }
            });

            playedSoundReference.Initialize(audioSource, despawnSequence);

            GetPlayedSoundsByType(type).Add(playedSoundReference);
            return playedSoundReference;
        }

        public void StopAllSoundsOfType(TSoundType type) {
            var playedSoundsOfType = GetPlayedSoundsByType(type);
            foreach (var playedSoundReference in playedSoundsOfType.ToArray()) {
                playedSoundReference.Stop();
            }
        }

        private void KillSoundImmediately(
            TSoundType type,
            AudioSource audioSource,
            GameObject audioSourceGameObject,
            PlayedSoundReference playedSoundReference
        ) {
            if (!GetPlayedSoundsByType(type).Remove(playedSoundReference)) {
                Debug.LogWarning($"Attempt to stop playing sound of type {type} that's not registered");
            }

            playedSoundReference.Stop();
            if (audioSource != null) {
                audioSource.Stop();
            }

            if (audioSourceGameObject != null) {
                _audioSourcePool.Despawn(audioSourceGameObject);
            }

            LeanClassPool<PlayedSoundReference>.Despawn(playedSoundReference);
        }

        private HashSet<PlayedSoundReference> GetPlayedSoundsByType(TSoundType type) {
            if (_activeSounds.TryGetValue(type, out var activeSoundsOfType))
                return activeSoundsOfType;

            activeSoundsOfType = new HashSet<PlayedSoundReference>();
            _activeSounds.Add(type, activeSoundsOfType);
            return activeSoundsOfType;
        }
    }
}
