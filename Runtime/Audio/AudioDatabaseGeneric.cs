using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Audio.Containers;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    /// <inheritdoc />
    /// <summary>
    /// Audio database that stores and retrieves sound and music containers
    /// </summary>
    /// <typeparam name="TMusicType">Music enum type</typeparam>
    /// <typeparam name="TSoundType">Sound enum type</typeparam>
    public abstract class AudioDatabaseGeneric<TMusicType, TSoundType> :
        SingletonScriptableObject<AudioDatabaseGeneric<TMusicType, TSoundType>>
        where TMusicType : unmanaged, Enum
        where TSoundType : unmanaged, Enum
    {
        /// <summary>
        /// Collection of sound containers configured for this database
        /// </summary>
        [field: SerializeField] public List<SoundContainer<TSoundType>> SoundContainers { get; private set; }

        /// <summary>
        /// Collection of music containers configured for this database
        /// </summary>
        [field: SerializeField] public List<MusicContainer<TMusicType>> MusicContainers { get; private set; }

        private SoundContainer<TSoundType>[] _soundLookup;
        private MusicContainer<TMusicType>[] _musicLookup;
        private int _maxSoundId;
        private int _maxMusicId;

        private bool _isInitialized;

        /// <summary>
        /// Initializes the database for optimized lookups. Must be called before use.
        /// </summary>
        [UsedImplicitly]
        public void Init()
        {
            BuildSoundLookup();
            BuildMusicLookup();

            _isInitialized = true;
        }

        private void BuildSoundLookup()
        {
            if (SoundContainers == null || SoundContainers.Count == 0)
            {
                _soundLookup = Array.Empty<SoundContainer<TSoundType>>();
                _maxSoundId = -1;
                return;
            }

            _maxSoundId = 0;
            foreach (var container in SoundContainers)
            {
                var id = container.GetId();
                if (id > _maxSoundId)
                    _maxSoundId = id;
            }

            _soundLookup = new SoundContainer<TSoundType>[_maxSoundId + 1];
            foreach (var container in SoundContainers)
                _soundLookup[container.GetId()] = container;
        }

        private void BuildMusicLookup()
        {
            if (MusicContainers == null || MusicContainers.Count == 0)
            {
                _musicLookup = Array.Empty<MusicContainer<TMusicType>>();
                _maxMusicId = -1;
                return;
            }

            _maxMusicId = 0;
            foreach (var container in MusicContainers)
            {
                var id = container.GetId();

                if (id > _maxMusicId)
                    _maxMusicId = id;
            }

            _musicLookup = new MusicContainer<TMusicType>[_maxMusicId + 1];
            foreach (var container in MusicContainers)
                _musicLookup[container.GetId()] = container;
        }

        private SoundContainer<TSoundType> GetSoundContainerLinear(TSoundType soundType)
        {
            if (SoundContainers == null)
                return null;

            var targetId = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var container in SoundContainers)
            {
                if (container.GetId() == targetId)
                    return container;
            }

            return null;
        }

        private MusicContainer<TMusicType> GetMusicContainerLinear(TMusicType musicType)
        {
            if (MusicContainers == null)
                return null;

            var targetId = UnsafeEnumConverter<TMusicType>.ToInt32(musicType);

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var container in MusicContainers)
            {
                if (container.GetId() == targetId)
                    return container;
            }

            return null;
        }

        internal SoundContainer<TSoundType> GetSoundContainer(TSoundType soundType)
        {
            if (_isInitialized is false)
                return GetSoundContainerLinear(soundType);

            var id = UnsafeEnumConverter<TSoundType>.ToInt32(soundType);
            return (uint)id <= _maxSoundId ? _soundLookup[id] : null;
        }

        internal MusicContainer<TMusicType> GetMusicContainer(TMusicType musicType)
        {
            if (_isInitialized is false)
                return GetMusicContainerLinear(musicType);

            var id = UnsafeEnumConverter<TMusicType>.ToInt32(musicType);
            return (uint)id <= _maxMusicId ? _musicLookup[id] : null;
        }
    }
}