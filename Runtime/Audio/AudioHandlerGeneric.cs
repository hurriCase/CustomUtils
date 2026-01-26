using System;
using System.Collections.Generic;
using System.Threading;
using CustomUtils.Runtime.Audio.Containers;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Storage;
using CustomUtils.Unsafe;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    public abstract class AudioHandlerGeneric<TMusicType, TSoundType> : MonoBehaviour,
        IAudioHandlerGeneric<TMusicType, TSoundType>
        where TMusicType : unmanaged, Enum
        where TSoundType : unmanaged, Enum
    {
        [SerializeField] protected AudioDatabaseGeneric<TMusicType, TSoundType> audioDatabaseGeneric;
        [SerializeField] protected AudioSource soundSourcePrefab;
        [SerializeField] protected AudioSource clipSource;
        [SerializeField] protected AudioSource musicSource;
        [SerializeField] protected AudioSource oneShotSource;
        [SerializeField] protected int soundPoolSize;
        [SerializeField] protected int maxPoolSize;

        public virtual PersistentReactiveProperty<float> MusicVolume { get; } = new();
        public virtual PersistentReactiveProperty<float> SoundVolume { get; } = new();

        private readonly Dictionary<int, float> _lastPlayedTimes = new();
        private readonly List<AliveAudioData<TSoundType>> _aliveAudios = new();
        private readonly List<AliveAudioData<TSoundType>> _audiosToRemove = new();

        private PoolHandler<AudioSource> _soundPool;
        private AudioData _currentMusicData;

        private const string MusicVolumeKey = "MusicVolumeKey";
        private const string SoundVolumeKey = "SoundVolumeKey";

        public virtual async UniTask InitAsync(
            float defaultMusicVolume = 1f,
            float defaultSoundVolume = 1f,
            CancellationToken cancellationToken = default)
        {
            await MusicVolume.InitAsync(MusicVolumeKey, destroyCancellationToken, defaultMusicVolume);
            await SoundVolume.InitAsync(SoundVolumeKey, destroyCancellationToken, defaultSoundVolume);

            _soundPool = new PoolHandler<AudioSource>();
            _soundPool.Init(soundSourcePrefab, soundPoolSize, maxPoolSize, parent: transform);

            audioDatabaseGeneric.Init();

            SoundVolume.SubscribeUntilDestroy(this, static (volume, self) => self.OnSoundVolumeChanged(volume));
            MusicVolume.SubscribeUntilDestroy(this, static (volume, self) => self.OnMusicVolumeChanged(volume));
        }

        public virtual AudioSource PlaySound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = audioDatabaseGeneric.GetSoundContainer(soundType);
            if (ShouldPlaySound(soundType, soundData) is false)
                return null;

            var soundTypeValue = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);
            _lastPlayedTimes[soundTypeValue] = Time.unscaledTime;

            var soundSource = _soundPool.Get();
            soundSource.clip = soundData.AudioData.AudioClip;
            soundSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            soundSource.volume = SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;

            soundSource.Play();

            var aliveData = new AliveAudioData<TSoundType>(soundType, soundSource);
            _aliveAudios.Add(aliveData);

            PlaySoundInternal(aliveData).Forget();

            return soundSource;
        }

        public virtual AudioSource PlayClip(AudioClip soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            clipSource.clip = soundType;
            clipSource.pitch = pitchModifier;
            clipSource.volume = SoundVolume.Value * volumeModifier;

            clipSource.Play();

            return clipSource;
        }

        public virtual void StopClip()
        {
            clipSource.Stop();
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

            _currentMusicData = data;

            return musicSource;
        }

        public virtual void StopMusic()
        {
            musicSource.Stop();
        }

        public virtual void StopSound(TSoundType soundType)
        {
            var soundTypeValueToRemove = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);

            _audiosToRemove.Clear();
            foreach (var audioData in _aliveAudios)
            {
                var soundTypeValue = UnsafeEnumConverter<TSoundType>.ToInt32(audioData.SoundType);

                if (soundTypeValue != soundTypeValueToRemove)
                    continue;

                audioData.AudioSource.Stop();
                _soundPool.Release(audioData.AudioSource);
                _audiosToRemove.Add(audioData);
            }

            foreach (var audioData in _audiosToRemove)
                _aliveAudios.Remove(audioData);
        }

        /// <summary>
        /// Called when sound volume changes to update all active sound sources
        /// </summary>
        /// <param name="soundVolume">New sound volume level</param>
        protected virtual void OnSoundVolumeChanged(float soundVolume)
        {
            foreach (var aliveAudioData in _aliveAudios)
                aliveAudioData.AudioSource.volume = soundVolume;
        }

        /// <summary>
        /// Called when music volume changes to update the music source
        /// </summary>
        /// <param name="musicVolume">New music volume level</param>
        protected virtual void OnMusicVolumeChanged(float musicVolume)
        {
            musicSource.volume = (_currentMusicData?.RandomVolume ?? 0) * musicVolume;
        }

        private bool ShouldPlaySound(TSoundType soundType, SoundContainer<TSoundType> soundData)
        {
            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return false;

            var soundValue = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);
            return soundData.Cooldown == 0 ||
                   _lastPlayedTimes.TryGetValue(soundValue, out var lastTime) is false ||
                   (Time.unscaledTime < lastTime + soundData.Cooldown) is false;
        }

        private async UniTask PlaySoundInternal(AliveAudioData<TSoundType> aliveData)
        {
            await UniTask.WaitForSeconds(aliveData.AudioSource.clip.length);

            _soundPool.Release(aliveData.AudioSource);
            _aliveAudios.Remove(aliveData);
        }

        protected virtual void OnDestroy()
        {
            MusicVolume.Dispose();
            SoundVolume.Dispose();
        }
    }
}