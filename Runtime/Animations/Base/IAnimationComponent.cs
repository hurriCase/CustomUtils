using System;
using JetBrains.Annotations;
using PrimeTween;

namespace CustomUtils.Runtime.Animations.Base
{
    /// <summary>
    /// Defines a component that can play and cancel state-based animations.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [UsedImplicitly]
    public interface IAnimationComponent<in TState>
        where TState : unmanaged, Enum
    {
        /// <summary>
        /// Plays the animation for the specified state.
        /// </summary>
        /// <param name="state">The animation state to play.</param>
        /// <param name="isInstant">If true, applies the animation immediately without tweening.</param>
        /// <returns>The tween controlling the animation, or default if instant.</returns>
        [UsedImplicitly]
        Tween PlayAnimation(TState state, bool isInstant);

        /// <summary>
        /// Cancels the currently playing animation.
        /// </summary>
        [UsedImplicitly]
        void CancelAnimation();
    }
}