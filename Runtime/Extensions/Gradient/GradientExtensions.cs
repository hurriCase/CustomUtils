using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.Extensions.Gradient
{
    internal static class GradientExtensions
    {
        private static readonly int _gradientStartColorProperty = Shader.PropertyToID("_GradientStartColor");
        private static readonly int _gradientEndColorProperty = Shader.PropertyToID("_GradientEndColor");
        private static readonly int _gradientDirectionProperty = Shader.PropertyToID("_GradientDirection");

        private static readonly Dictionary<string, Material> _gradientMaterials = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearMaterialCache()
        {
            foreach (var material in _gradientMaterials.Values)
            {
                if (!material)
                    continue;

                if (Application.isPlaying)
                    Object.Destroy(material);
                else
                    Object.DestroyImmediate(material);
            }

            _gradientMaterials.Clear();
        }

        /// <summary>
        /// Applies a gradient to an Image component
        /// </summary>
        /// <param name="targetImage">The Image component to apply the gradient to</param>
        /// <param name="gradient">The gradient to apply</param>
        /// <param name="direction">The gradient direction (Horizontal or Vertical)</param>
        internal static void ApplyGradient(this Image targetImage, UnityEngine.Gradient gradient,
            GradientDirection direction = GradientDirection.Horizontal)
        {
            if (gradient == null || gradient.colorKeys.Length < 1)
            {
                Debug.LogError("[GradientExtensions::ApplyGradient] Invalid gradient provided." +
                               " Ensure it has at least one color key.");
                return;
            }

            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            var materialKey = $"IMG_{GenerateGradientKey(gradient, direction)}";

            if (_gradientMaterials.TryGetValue(materialKey, out var gradientMaterial) is false || !gradientMaterial)
            {
                var imageGradientShader = Shader.Find("Custom/ImageGradientShader");
                if (!imageGradientShader)
                {
                    Debug.LogError("[GradientExtensions::ApplyGradient] Image GradientHelpers Shader not found.");
                    return;
                }

                gradientMaterial = new Material(imageGradientShader);
                _gradientMaterials[materialKey] = gradientMaterial;

                gradientMaterial.SetColor(_gradientStartColorProperty, startColor);
                gradientMaterial.SetColor(_gradientEndColorProperty, endColor);
                gradientMaterial.SetFloat(_gradientDirectionProperty, (float)direction);
            }

            targetImage.material = gradientMaterial;
            targetImage.color = Color.white;
            targetImage.SetMaterialDirty();
        }

        /// <summary>
        /// Applies a gradient to a TextMeshProUGUI component
        /// </summary>
        /// <param name="targetText">The TextMeshProUGUI component to apply the gradient to</param>
        /// <param name="gradient">The gradient to apply</param>
        /// <param name="direction">The gradient direction (Horizontal or Vertical)</param>
        internal static void ApplyGradient(this TextMeshProUGUI targetText, UnityEngine.Gradient gradient,
            GradientDirection direction = GradientDirection.Horizontal)
        {
            if (gradient == null || gradient.colorKeys.Length < 1)
            {
                Debug.LogError("Invalid gradient provided. Ensure it has at least one color key.");
                return;
            }

            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            var materialKey = $"TMP_{GenerateGradientKey(gradient, direction)}";

            if (_gradientMaterials.TryGetValue(materialKey, out var gradientMaterial) is false || !gradientMaterial)
            {
                var textGradientShader = Shader.Find("Custom/TextGradientShader");
                if (!textGradientShader)
                {
                    Debug.LogError("[GradientExtensions::ApplyGradient] Text GradientHelpers Shader not found.");
                    return;
                }

                gradientMaterial = new Material(textGradientShader);
                _gradientMaterials[materialKey] = gradientMaterial;

                gradientMaterial.SetColor(_gradientStartColorProperty, startColor);
                gradientMaterial.SetColor(_gradientEndColorProperty, endColor);
                gradientMaterial.SetFloat(_gradientDirectionProperty, (float)direction);
            }

            targetText.fontMaterial = gradientMaterial;
            targetText.SetMaterialDirty();
        }

        /// <summary>
        /// Extracts the current gradient from an Image component if it has a gradient material
        /// </summary>
        /// <param name="image">The Image to extract the gradient from</param>
        /// <returns>A new Gradient object if gradient material is applied, null otherwise</returns>
        internal static UnityEngine.Gradient GetAppliedGradient(this Image image)
        {
            if (!image || !image.material)
                return null;

            if (image.material.HasProperty(_gradientStartColorProperty) is false ||
                image.material.HasProperty(_gradientEndColorProperty) is false)
                return null;

            var startColor = image.material.GetColor(_gradientStartColorProperty);
            var endColor = image.material.GetColor(_gradientEndColorProperty);

            return CreateGradientFromColors(startColor, endColor);
        }

        /// <summary>
        /// Extracts the current gradient from a TextMeshProUGUI component if it has a gradient material
        /// </summary>
        /// <param name="text">The TextMeshProUGUI to extract the gradient from</param>
        /// <returns>A new Gradient object if gradient material is applied, null otherwise</returns>
        internal static UnityEngine.Gradient GetAppliedGradient(this TextMeshProUGUI text)
        {
            if (!text || !text.fontMaterial)
                return null;

            if (text.fontMaterial.HasProperty(_gradientStartColorProperty) is false ||
                text.fontMaterial.HasProperty(_gradientEndColorProperty) is false)
                return null;

            var startColor = text.fontMaterial.GetColor(_gradientStartColorProperty);
            var endColor = text.fontMaterial.GetColor(_gradientEndColorProperty);

            return CreateGradientFromColors(startColor, endColor);
        }

        /// <summary>
        /// Compares the current applied gradient on an Image with the provided gradient
        /// </summary>
        /// <param name="image">The Image to check</param>
        /// <param name="gradient">The gradient to compare with</param>
        /// <returns>True if the Image has the same gradient applied, false otherwise</returns>
        internal static bool CompareGradient(this Image image, UnityEngine.Gradient gradient)
        {
            if (!image || gradient == null)
                return false;

            var currentGradient = image.GetAppliedGradient();
            return GradientsEqual(currentGradient, gradient);
        }

        /// <summary>
        /// Compares the current applied gradient on a TextMeshProUGUI with the provided gradient
        /// </summary>
        /// <param name="text">The TextMeshProUGUI to check</param>
        /// <param name="gradient">The gradient to compare with</param>
        /// <returns>True if the Text has the same gradient applied, false otherwise</returns>
        internal static bool CompareGradient(this TextMeshProUGUI text, UnityEngine.Gradient gradient)
        {
            if (!text || gradient == null)
                return false;

            var currentGradient = text.GetAppliedGradient();
            return GradientsEqual(currentGradient, gradient);
        }

        /// <summary>
        /// Creates a new Gradient object from start and end colors
        /// </summary>
        /// <param name="startColor">The start color</param>
        /// <param name="endColor">The end color</param>
        /// <returns>A new Gradient object with the specified colors</returns>
        private static UnityEngine.Gradient CreateGradientFromColors(Color startColor, Color endColor)
        {
            var gradient = new UnityEngine.Gradient();

            var colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(startColor, 0f);
            colorKeys[1] = new GradientColorKey(endColor, 1f);

            var alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(startColor.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(endColor.a, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);

            return gradient;
        }

        /// <summary>
        /// Compares two gradients to check if they have exactly the same start and end colors
        /// </summary>
        /// <param name="gradient1">First gradient</param>
        /// <param name="gradient2">Second gradient</param>
        /// <returns>True if gradients are identical, false otherwise</returns>
        private static bool GradientsEqual(UnityEngine.Gradient gradient1, UnityEngine.Gradient gradient2)
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

        private static string GenerateGradientKey(UnityEngine.Gradient gradient, GradientDirection direction)
        {
            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            return $"{startColor.r:F2}_{startColor.g:F2}_{startColor.b:F2}_{startColor.a:F2}_" +
                   $"{endColor.r:F2}_{endColor.g:F2}_{endColor.b:F2}_{endColor.a:F2}_" +
                   $"{(int)direction}";
        }
    }
}