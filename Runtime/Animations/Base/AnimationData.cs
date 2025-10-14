using System;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Base
{
    /// <summary>
    /// Contains the target value and tween settings for an animation.
    /// </summary>
    /// <typeparam name="TValue">The type of value being animated.</typeparam>
    [UsedImplicitly]
    [Serializable]
    public struct AnimationData<TValue>
    {
        [field: SerializeField] public TValue Value { get; private set; }
        [field: SerializeField] public TweenSettings TweenSettings { get; private set; }
    }
}