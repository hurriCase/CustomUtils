using System;
using CustomUtils.Runtime.Animations.Base;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations
{
    /// <summary>
    /// Animates the alpha value of a CanvasGroup based on state.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [UsedImplicitly]
    [Serializable]
    public sealed class AlphaAnimation<TState> : AnimationBase<TState, float>
        where TState : unmanaged, Enum
    {
        [SerializeField] private CanvasGroup _target;

        protected override void SetValueInstant(float value)
        {
            _target.alpha = value;
        }

        protected override Tween CreateTween(AnimationData<float> animationData)
            => Tween.Alpha(_target, animationData.Value, animationData.TweenSettings);
    }
}