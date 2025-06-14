using System;
using System.Collections.Generic;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.Storage;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using JetBrains.Annotations;
using PrimeTween;
using R3;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Audio
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for handling audio playback including music and sound effects
    /// </summary>
    /// <typeparam name="TMusicType">Enum type for music categories</typeparam>
    /// <typeparam name="TSoundType">Enum type for sound effect categories</typeparam>
    [UsedImplicitly]
    public abstract class AudioHandlerBase<TMusicType, TSoundType> :
        PersistentSingletonBehavior<AudioHandlerBase<TMusicType, TSoundType>>
        where TMusicType : unmanaged, Enum
        where TSoundType : unmanaged, Enum
    {
        [SerializeField] protected AudioSource _soundSourcePrefab;
        [SerializeField] protected AudioSource _musicSource;
        [SerializeField] protected AudioSource _oneShotSource;
        [SerializeField] protected int _defaultSoundPoolCount = 3;

        protected virtual PersistentReactiveProperty<float> MusicVolume { get; } = new("music_volume_key", 1);
        protected virtual PersistentReactiveProperty<float> SoundVolume { get; } = new("sound_volume_key", 1);

        private AudioDatabaseGeneric<TMusicType, TSoundType> AudioDatabaseGeneric =>
            AudioDatabaseGeneric<TMusicType, TSoundType>.Instance;

        private readonly Dictionary<int, float> _lastPlayedTimes = new();
        private readonly SortedDictionary<float, AliveAudioData<TSoundType>> _sortedAliveAudioData = new();

        private PoolHandler<AudioSource> _soundPool;
        private IDisposable _disposable;

        /// <summary>
        /// Initializes the audio handler with pooled audio sources and volume subscriptions
        /// </summary>
        [UsedImplicitly]
        public virtual void Init()
        {
            _soundPool = new PoolHandler<AudioSource>();
            _soundPool.Init(_soundSourcePrefab, _defaultSoundPoolCount, _defaultSoundPoolCount * 5);

            var soundDisposable = SoundVolume
                .Subscribe(this, (handler, volume) => handler.OnSoundVolumeChanged(volume));

            var musicDisposable = MusicVolume
                .Subscribe(this, (handler, volume) => handler.OnMusicVolumeChanged(volume));

            _disposable = Disposable.Combine(soundDisposable, musicDisposable);
        }

        /// <summary>
        /// Changes the global sound effects volume
        /// </summary>
        /// <param name="volume">Volume level from 0 to 1</param>
        [UsedImplicitly]
        public virtual void ChangeSoundVolume(float volume) => SoundVolume.Value = volume;

        /// <summary>
        /// Changes the global music volume
        /// </summary>
        /// <param name="volume">Volume level from 0 to 1</param>
        [UsedImplicitly]
        public virtual void ChangeMusicVolume(float volume) => MusicVolume.Value = volume;

        /// <summary>
        /// Plays a sound with the specified parameters
        /// </summary>
        /// <param name="soundType">Type of sound to play</param>
        /// <param name="volumeModifier">Volume multiplier</param>
        /// <param name="pitchModifier">Pitch multiplier</param>
        /// <returns>AudioSource playing the sound, or null if failed</returns>
        [UsedImplicitly]
        public virtual AudioSource PlaySound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = AudioDatabaseGeneric.GetSoundContainer(soundType);

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return null;

            var soundId = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);
            if (soundData.Cooldown != 0 &&
                _lastPlayedTimes.TryGetValue(soundId, out var lastTime) &&
                Time.unscaledTime < lastTime + soundData.Cooldown)
                return null;

            _lastPlayedTimes[soundId] = Time.unscaledTime;

            var soundSource = _soundPool.Get();
            soundSource.clip = soundData.AudioData.AudioClip;
            soundSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            soundSource.volume = SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;

            soundSource.Play();

            _sortedAliveAudioData.Add(
                soundData.AudioData.AudioClip.length,
                new AliveAudioData<TSoundType> { SoundType = soundType, AudioSource = soundSource });

            Tween.Delay(this, soundData.AudioData.AudioClip.length,
                handler =>
                {
                    var aliveData =
                        handler._sortedAliveAudioData.AsValueEnumerable().First();

                    handler._soundPool.Release(aliveData.Value.AudioSource);
                    handler._sortedAliveAudioData.Remove(aliveData.Key);
                });

            return soundSource;
        }

        /// <summary>
        /// Stops all instances of the specified sound type
        /// </summary>
        /// <param name="soundType">Type of sound to stop</param>
        [UsedImplicitly]
        public virtual void StopSound(TSoundType soundType)
        {
            var soundId = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);

            var toRemove = new List<float>();
            foreach (var (time, audioData) in _sortedAliveAudioData)
            {
                var audioSoundId = UnsafeEnumConverter<TSoundType>.ToInt32(audioData.SoundType);

                if (audioSoundId != soundId)
                    continue;

                audioData.AudioSource.Stop();
                _soundPool.Release(audioData.AudioSource);
                toRemove.Add(time);
            }

            foreach (var key in toRemove)
                _sortedAliveAudioData.Remove(key);
        }

        /// <summary>
        /// Plays a one-shot sound that doesn't need to be tracked or stopped
        /// </summary>
        /// <param name="soundType">Type of sound to play</param>
        /// <param name="volumeModifier">Volume multiplier</param>
        /// <param name="pitchModifier">Pitch multiplier</param>
        [UsedImplicitly]
        public virtual void PlayOneShotSound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = AudioDatabaseGeneric.GetSoundContainer(soundType);

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return;

            _oneShotSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            _oneShotSource.volume = SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;
            _oneShotSource.PlayOneShot(soundData.AudioData.AudioClip);
        }

        /// <summary>
        /// Plays music of the specified type
        /// </summary>
        /// <param name="musicType">Type of music to play</param>
        /// <returns>AudioSource playing the music, or null if failed</returns>
        [UsedImplicitly]
        public virtual AudioSource PlayMusic(TMusicType musicType)
        {
            var musicData = AudioDatabaseGeneric.GetMusicContainer(musicType);
            return musicData?.AudioData == null ? null : PlayMusic(musicData.AudioData);
        }

        /// <summary>
        /// Plays music from the specified audio data
        /// </summary>
        /// <param name="data">Audio data to play</param>
        /// <returns>AudioSource playing the music, or null if failed</returns>
        [UsedImplicitly]
        public virtual AudioSource PlayMusic(AudioData data)
        {
            if (data == null || !data.AudioClip)
                return null;

            _musicSource.clip = data.AudioClip;
            _musicSource.pitch = data.RandomPitch;
            _musicSource.volume = data.RandomVolume * MusicVolume.Value;
            _musicSource.Play();

            return _musicSource;
        }

        /// <summary>
        /// Called when sound volume changes to update all active sound sources
        /// </summary>
        /// <param name="soundVolume">New sound volume level</param>
        protected virtual void OnSoundVolumeChanged(float soundVolume)
        {
            foreach (var aliveAudioData in _sortedAliveAudioData.Values)
                aliveAudioData.AudioSource.volume *= soundVolume;
        }

        /// <summary>
        /// Called when music volume changes to update the music source
        /// </summary>
        /// <param name="musicVolume">New music volume level</param>
        protected virtual void OnMusicVolumeChanged(float musicVolume)
        {
            _musicSource.volume *= musicVolume;
        }

        /// <summary>
        /// Cleans up subscriptions when the object is destroyed
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _disposable?.Dispose();
        }
    }
}