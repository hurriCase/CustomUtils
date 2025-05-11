using System;
using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.Audio.Containers;
using CustomUtils.Runtime.CustomTypes.Singletons;
using PrimeTween;
using R3;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.Audio
{
    [Resource(
        resourcePath: ResourcePaths.DontDestroyOnLoadPath,
        name: ResourcePaths.PrefabPrefix + nameof(AudioHandler)
    )]
    public sealed class AudioHandler : PersistentSingletonBehavior<AudioHandler>
    {
        [SerializeField] private AudioDatabase _audioDatabase;
        [SerializeField] private AudioSource _soundSourcePrefab;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _oneShotSource;
        [SerializeField] private int _defaultSoundPoolCount = 3;

        private readonly SortedDictionary<float, AliveAudioData> _sortedAliveAudioData = new();

        private PoolHandler<AudioSource> _soundPool;
        private float _lastTimePlayed = -1;

        private AudioRepository _audioRepository;

        private IDisposable _disposable;

        public void Init(AudioRepository audioRepository)
        {
            _soundPool = new PoolHandler<AudioSource>();
            _soundPool.Init(_soundSourcePrefab, _defaultSoundPoolCount, _defaultSoundPoolCount * 5);

            _audioRepository = audioRepository;

            var soundDisposable = _audioRepository.SoundVolume.Subscribe(OnSoundVolumeChanged);
            var musicDisposable = _audioRepository.MusicVolume.Subscribe(OnMusicVolumeChanged);

            _disposable = Disposable.Combine(soundDisposable, musicDisposable);
        }

        public AudioSource PlaySound(SoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = _audioDatabase.GetSoundContainer(soundType);

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return null;

            if (soundData.Cooldown != 0 &&
                (Time.unscaledTime > _lastTimePlayed + soundData.Cooldown) is false)
                return null;

            _lastTimePlayed = Time.unscaledTime;

            var soundSource = _soundPool.Get();
            soundSource.clip = soundData.AudioData.AudioClip;
            soundSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            soundSource.volume = _audioRepository.SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;

            soundSource.Play();

            _sortedAliveAudioData.Add(soundData.AudioData.AudioClip.length, new AliveAudioData
                { SoundType = soundType, AudioSource = soundSource });

            Tween.Delay(this, soundData.AudioData.AudioClip.length,
                handler =>
                {
                    var aliveData = handler._sortedAliveAudioData.AsValueEnumerable().First();
                    handler._soundPool.Release(aliveData.Value.AudioSource);
                });

            return soundSource;
        }

        public void StopSound(SoundType soundType)
        {
            foreach (var audioData in _sortedAliveAudioData.Values)
            {
                if (audioData.SoundType != soundType)
                    continue;

                audioData.AudioSource.Stop();
                _soundPool.Release(audioData.AudioSource);
            }
        }

        public void PlayOneShotSound(SoundType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = _audioDatabase.GetSoundContainer(soundType);

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return;

            _oneShotSource.pitch = pitchModifier * soundData.AudioData.RandomPitch;
            _oneShotSource.volume = _audioRepository.SoundVolume.Value * volumeModifier * soundData.AudioData.RandomVolume;

            _oneShotSource.PlayOneShot(soundData.AudioData.AudioClip);
        }

        internal AudioSource PlayMusic(MusicType musicType)
        {
            var musicData = _audioDatabase.GeMusicContainer(musicType);

            return musicData?.AudioData == null ? null : PlayMusic(musicData.AudioData);
        }

        public AudioSource PlayMusic(AudioData data)
        {
            if (data == null || !data.AudioClip)
                return null;

            _musicSource.clip = data.AudioClip;
            _musicSource.pitch = data.RandomPitch;
            _musicSource.volume = data.RandomVolume * _audioRepository.MusicVolume.Value;

            _musicSource.Play();

            return _musicSource;
        }

        private void OnSoundVolumeChanged(float soundVolume)
        {
            foreach (var aliveAudioData in _sortedAliveAudioData.Values)
                aliveAudioData.AudioSource.volume *= soundVolume;
        }

        private void OnMusicVolumeChanged(float soundVolume)
        {
            _musicSource.volume *= soundVolume;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _disposable.Dispose();
        }
    }
}