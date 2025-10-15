using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using PrimeTween;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Base
{
    [Serializable]
    public abstract class AnimationBase<TState, TValue, TAnimationSettings> : IAnimation<TState>
        where TState : unmanaged, Enum
        where TAnimationSettings : AnimationSettings<TValue>
    {
        [SerializeField] protected EnumArray<TState, TAnimationSettings> states;

        private Tween _currentAnimation;

        public Tween PlayAnimation(TState state, bool isInstant = false)
        {
            var currentState = states[state];

            if (isInstant)
            {
                SetValueInstant(currentState.Value);
                return default;
            }

            if (_currentAnimation.isAlive)
                _currentAnimation.Stop();

            return _currentAnimation = CreateTween(currentState);
        }

        public void CancelAnimation()
        {
            _currentAnimation.Stop();
        }

        protected abstract void SetValueInstant(TValue value);
        protected abstract Tween CreateTween(TAnimationSettings animationSettings);
    }
}