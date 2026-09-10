using System;
using CustomUtils.Runtime.Animations.Base.Settings;
using PrimeTween;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Runtime.Animations.Base
{
    [Serializable]
    public abstract class DelayedAnimationBase<TTarget, TContent, TState> : StatefulAnimationBase<TState>
        where TState : unmanaged, Enum
		where TTarget : Object
    {
        [SerializeField] protected TTarget target;
        [SerializeField] private DelayedAnimationSettingsBase<TState, TContent> _animationSettings;

        protected TContent targetContent;

        protected override Tween OnPlayAnimation(TState state, bool isInstant)
        {
            if (CurrentAnimation.isAlive)
                CurrentAnimation.Stop();

            if (isInstant && _animationSettings.SkipWhenInstant)
                return default;

            targetContent = _animationSettings.States[state];

            if (isInstant)
            {
                UpdateState();
                return default;
            }

            return Tween.Delay(
                target,
                _animationSettings.Delay,
                _ => UpdateState(),
                _animationSettings.UseUnscaledTime);
        }

        private void UpdateState()
        {
            if (target)
                OnUpdateState(target, targetContent);
        }

        protected abstract void OnUpdateState(TTarget target, TContent targetContent);
    }
}