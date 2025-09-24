using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.GradientHelpers.Base
{
    public abstract class GradientEffectBase<TComponent> where TComponent : Component
    {
        internal void ApplyGradient(
            [NotNull] TComponent component,
            [NotNull] Gradient gradient,
            GradientDirection direction)
        {
            if (gradient.colorKeys.Length < 1)
            {
                Debug.LogError("[GradientEffectBase::ApplyGradient] Invalid gradient provided." +
                               " Ensure it has at least one color key.");
                return;
            }

            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            ApplyGradient(component, startColor, endColor, direction);
        }

        protected abstract void ApplyGradient(
            TComponent component,
            Color startColor,
            Color endColor,
            GradientDirection direction);

        public abstract void ClearGradient(TComponent component);
    }
}