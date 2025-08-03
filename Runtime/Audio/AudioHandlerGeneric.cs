using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Storage;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using Cysharp.Threading.Tasks;
using PrimeTween;
using R3;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Audio
{
    public abstract class AudioHandlerGeneric<TMusicType, TSoundType> : MonoBehaviour,
        IAudioHandlerGeneric<TMusicType, TSoundType>
        where TMusicType : unmanaged, Enum
        where TSoundType : unmanaged, Enum
    {
        [SerializeField] protected AudioDatabaseGeneric<TMusicType, TSoundType> audioDatabaseGeneric;
        [SerializeField] protected AudioSource soundSourcePrefab;
        [SerializeField] protected AudioSource musicSource;
        [SerializeField] protected AudioSource oneShotSource;
        [SerializeField] protected int defaultSoundPoolCount = 3;

        public virtual PersistentReactiveProperty<float> MusicVolume { get; } = new();

        public virtual PersistentReactiveProperty<float> SoundVolume { get; } = new();

        private readonly Dictionary<int, float> _lastPlayedTimes = new();
        private readonly SortedDictionary<float, AliveAudioData<TSoundType>> _sortedAliveAudioData = new();

        private PoolHandler<AudioSource> _soundPool;
        private IDisposable _disposable;

        private const string MusicVolumeKey = "MusicVolumeKey";
        private const string SoundVolumeKey = "SoundVolumeKey";

        public virtual async UniTask InitAsync(float defaultMusicVolume = 1f, float defaultSoundVolume = 1f)
        {
            await MusicVolume.InitAsync(MusicVolumeKey, destroyCancellationToken, defaultMusicVolume);
            await SoundVolume.InitAsync(SoundVolumeKey, destroyCancellationToken, defaultSoundVolume);

            _soundPool = new PoolHandler<AudioSource>();
            _soundPool.Init(soundSourcePrefab, defaultSoundPoolCount, defaultSoundPoolCount * 5);

            audioDatabaseGeneric.Init();

            var soundDisposable = SoundVolume
                .Subscribe(this, (volume, handler) => handler.OnSoundVolumeChanged(volume));

            var musicDisposable = MusicVolume
                .Subscribe(this, (volume, handler) => handler.OnMusicVolumeChanged(volume));

            _disposable = Disposable.Combine(soundDisposable, musicDisposable);
        }

        public virtual AudioSource PlaySound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = audioDatabaseGeneric.GetSoundContainer(soundType);

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

        public virtual void PlayOneShotSound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = audioDatabaseGeneric.GetSoundContainer(soundType);

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return;

            oneShotSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            oneShotSource.volume = SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;
            oneShotSource.PlayOneShot(soundData.AudioData.AudioClip);
        }

        public virtual AudioSource PlayMusic(TMusicType musicType)
        {
            var musicData = audioDatabaseGeneric.GetMusicContainer(musicType);
            return musicData?.AudioData == null ? null : PlayMusic(musicData.AudioData);
        }

        public virtual AudioSource PlayMusic(AudioData data)
        {
            if (data == null || !data.AudioClip)
                return null;

            musicSource.clip = data.AudioClip;
            musicSource.pitch = data.RandomPitch;
            musicSource.volume = data.RandomVolume * MusicVolume.Value;
            musicSource.Play();

            return musicSource;
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
            musicSource.volume *= musicVolume;
        }

        /// <summary>
        /// Cleans up subscriptions when the object is destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}