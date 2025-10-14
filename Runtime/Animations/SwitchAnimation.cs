using System;
using CustomUtils.Runtime.Animations.Base;
using CustomUtils.Runtime.Extensions;
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations
{
    /// <summary>
    /// Switches between CanvasGroup components based on state, hiding inactive groups and showing the active one.
    /// </summary>
    /// <typeparam name="TState">The enum type representing animation states.</typeparam>
    [UsedImplicitly]
    [Serializable]
    public sealed class SwitchAnimation<TState> : AnimationBase<TState, CanvasGroup>
        where TState : unmanaged, Enum
    {
        private CanvasGroup _currentCanvasGroup;

        protected override void SetValueInstant(CanvasGroup canvasGroup)
        {
            _currentCanvasGroup = canvasGroup;

            HideOther();
            ShowCurrent();
        }

        protected override Tween CreateTween(AnimationData<CanvasGroup> animationData)
        {
            _currentCanvasGroup = animationData.Value;

            HideOther();

            return Tween.Alpha(animationData.Value, 1f, animationData.TweenSettings)
                .OnComplete(this, self => self.ShowCurrent());
        }

        private void HideOther()
        {
            foreach (var state in states)
            {
                state.Value.AsNullable()?.Hide();
                state.Value.AsNullable()?.SetActive(false);
            }
        }

        private void ShowCurrent()
        {
            _currentCanvasGroup.Show();
            _currentCanvasGroup.SetActive(true);
        }
    }
}