﻿using System;
using CustomUtils.Runtime.Animations.Base;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations
{
    /// <summary>
    /// Animates the pivot of a RectTransform based on state.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [UsedImplicitly]
    [Serializable]
    public sealed class PivotAnimation<TState> : AnimationBase<TState, Vector2>
        where TState : unmanaged, Enum
    {
        [SerializeField] private RectTransform _target;

        protected override void SetValueInstant(Vector2 value)
        {
            _target.pivot = value;
        }

        protected override Tween CreateTween(AnimationData<Vector2> animationData)
            => Tween.UIPivot(_target, animationData.Value, animationData.TweenSettings);
    }
}