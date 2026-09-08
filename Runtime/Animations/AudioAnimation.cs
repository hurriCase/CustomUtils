using System;
using CustomUtils.Runtime.Animations.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Animations
{
    /// <inheritdoc />
    /// <summary>
    /// Plays an AudioClip on an AudioSource component based on state, with optional delay.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [PublicAPI]
    [Serializable]
    public sealed class AudioAnimation<TState> : DelayedAnimationBase<AudioSource, AudioClip, TState>
        where TState : unmanaged, Enum
    {
        protected override void OnUpdateState(AudioSource audioSource, AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}