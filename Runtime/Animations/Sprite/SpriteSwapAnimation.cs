using System;
using CustomUtils.Runtime.Animations.Base;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace CustomUtils.Runtime.Animations.Sprite
{
    /// <inheritdoc />
    /// <summary>
    /// Swaps the sprite of an Image component based on state, with optional delay.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [PublicAPI]
    [Serializable]
    public sealed class SpriteSwapAnimation<TState> : DelayedAnimationBase<Image, UnityEngine.Sprite, TState>
        where TState : unmanaged, Enum
    {
        protected override void OnUpdateState(Image image, UnityEngine.Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}