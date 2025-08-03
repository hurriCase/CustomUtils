using System;
using System.Threading;
using CustomUtils.Runtime.Storage;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    /// <summary>
    /// Base class for handling audio playback including music and sound effects
    /// </summary>
    /// <typeparam name="TMusicType">Enum type for music categories</typeparam>
    /// <typeparam name="TSoundType">Enum type for sound effect categories</typeparam>
    [UsedImplicitly]
    public interface IAudioHandlerGeneric<in TMusicType, in TSoundType>
        where TMusicType : unmanaged, Enum
        where TSoundType : unmanaged, Enum
    {
        /// <summary>
        /// Gets the reactive property for music volume control.
        /// Value range is typically 0.0 to 1.0, where 0 is muted and 1 is full volume.
        /// Changes to this property will automatically persist to storage and update all active music sources.
        /// </summary>
        [UsedImplicitly]
        PersistentReactiveProperty<float> MusicVolume { get; }

        /// <summary>
        /// Gets the reactive property for sound effects volume control.
        /// Value range is typically 0.0 to 1.0, where 0 is muted and 1 is full volume.
        /// Changes to this property will automatically persist to storage and update all active sound sources.
        /// </summary>
        [UsedImplicitly]
        PersistentReactiveProperty<float> SoundVolume { get; }

        /// <summary>
        /// Initializes the audio handler with pooled audio sources and volume subscriptions
        /// </summary>
        [UsedImplicitly]
        UniTask InitAsync(
            float defaultMusicVolume = 1f,
            float defaultSoundVolume = 1f,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Plays a sound with the specified parameters
        /// </summary>
        /// <param name="soundType">Type of sound to play</param>
        /// <param name="volumeModifier">Volume multiplier</param>
        /// <param name="pitchModifier">Pitch multiplier</param>
        /// <returns>AudioSource playing the sound, or null if failed</returns>
        [UsedImplicitly]
        AudioSource PlaySound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1);

        /// <summary>
        /// Stops all instances of the specified sound type
        /// </summary>
        /// <param name="soundType">Type of sound to stop</param>
        [UsedImplicitly]
        void StopSound(TSoundType soundType);

        /// <summary>
        /// Plays a one-shot sound that doesn't need to be tracked or stopped
        /// </summary>
        /// <param name="soundType">Type of sound to play</param>
        /// <param name="volumeModifier">Volume multiplier</param>
        /// <param name="pitchModifier">Pitch multiplier</param>
        [UsedImplicitly]
        void PlayOneShotSound(TSoundType soundType, float volumeModifier = 1, float pitchModifier = 1);

        /// <summary>
        /// Plays music of the specified type
        /// </summary>
        /// <param name="musicType">Type of music to play</param>
        /// <returns>AudioSource playing the music, or null if failed</returns>
        [UsedImplicitly]
        AudioSource PlayMusic(TMusicType musicType);

        /// <summary>
        /// Plays music from the specified audio data
        /// </summary>
        /// <param name="data">Audio data to play</param>
        /// <returns>AudioSource playing the music, or null if failed</returns>
        [UsedImplicitly]
        AudioSource PlayMusic(AudioData data);
    }
}