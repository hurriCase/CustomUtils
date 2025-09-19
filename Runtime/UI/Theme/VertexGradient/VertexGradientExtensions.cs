using CustomUtils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.VertexGradient
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

        internal static void ClearVertexGradient(this Graphic graphic)
        {
            if (graphic.TryGetComponent<VertexGradientEffect>(out var gradientEffect) is false)
                return;

            if (Application.isPlaying)
                Object.Destroy(gradientEffect);
            else
                Object.DestroyImmediate(gradientEffect);

            graphic.SetVerticesDirty();
        }

        internal static bool CompareVertexGradient(this Graphic graphic, Gradient gradient)
        {
            if (!graphic || gradient == null)
                return false;

            var currentGradient = graphic.GetVertexGradient();
            return GradientsEqual(currentGradient, gradient);
        }

        private static Gradient GetVertexGradient(this Component graphic)
        {
            if (graphic.TryGetComponent<VertexGradientEffect>(out var gradientEffect) is false)
                return null;

            return gradientEffect.GetCurrentGradient();
        }

        private static bool GradientsEqual(Gradient gradient1, Gradient gradient2)
        {
            if (gradient1 == null || gradient2 == null)
                return false;

            if (gradient1.colorKeys.Length < 2 || gradient2.colorKeys.Length < 2)
                return false;

            var start1 = gradient1.colorKeys[0].color;
            var start2 = gradient2.colorKeys[0].color;
            if (start1 != start2)
                return false;

            var end1 = gradient1.colorKeys[^1].color;
            var end2 = gradient2.colorKeys[^1].color;
            return end1 == end2;
        }
    }
}