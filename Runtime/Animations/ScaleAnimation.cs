using System;
using CustomUtils.Runtime.Animations.Base;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations
{
    /// <summary>
    /// Animates the local scale of a Transform based on state.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [UsedImplicitly]
    [Serializable]
    public sealed class ScaleAnimation<TState> : AnimationBase<TState, Vector3>
        where TState : unmanaged, Enum
    {
        [SerializeField] private Transform _target;

        protected override void SetValueInstant(Vector3 value)
        {
            _target.localScale = value;
        }

        protected override Tween CreateTween(AnimationData<Vector3> animationData)
            => Tween.Scale(_target, animationData.Value, animationData.TweenSettings);
    }
}