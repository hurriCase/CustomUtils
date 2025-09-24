using CustomUtils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.ImageVertexGradient
{
    internal static class VertexGradientExtensions
    {
        internal static void ApplyVertexGradient(
            [NotNull] this Graphic graphic,
            [NotNull] Gradient gradient,
            GradientDirection direction)
        {
            if (gradient.colorKeys.Length < 1)
            {
                Debug.LogError("[VertexGradientExtensions::ApplyVertexGradient] Invalid gradient provided." +
                               " Ensure it has at least one color key.");
                return;
            }

            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            var gradientEffect = graphic.GetOrAddComponent<VertexGradientEffect>();

            gradientEffect.SetGradient(startColor, endColor, direction);
            graphic.SetVerticesDirty();
        }

        internal static void ClearVertexGradient([NotNull] this Graphic graphic)
        {
            if (graphic.TryGetComponent<VertexGradientEffect>(out var gradientEffect) is false)
                return;

            if (Application.isPlaying)
                Object.Destroy(gradientEffect);
            else
                Object.DestroyImmediate(gradientEffect);

            graphic.SetVerticesDirty();
        }
    }
}