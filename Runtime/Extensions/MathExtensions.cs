using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for mathematical operations or evaluations.
    /// </summary>
    [UsedImplicitly]
    public static class MathExtensions
    {
        private const float LowerThreshold = 0.1f;
        private const float UpperThreshold = 1000f;

        /// <summary>
        /// Determines if the given float value is within a reasonable range.
        /// </summary>
        /// <param name="value">The float value to evaluate.</param>
        /// <returns>True if the value is greater than the lower threshold and less than the upper threshold;
        /// otherwise, false.</returns>
        [UsedImplicitly]
        public static bool IsReasonable(this float value) => value is > LowerThreshold and < UpperThreshold;

        /// <summary>
        /// Clamps all components of the Vector4 instance to ensure they are non-negative.
        /// </summary>
        /// <param name="vector4">The Vector4 instance to be modified.</param>
        /// <returns>A new Vector4 instance with all components clamped to zero or higher.</returns>
        [UsedImplicitly]
        public static Vector4 ClampToPositive(this Vector4 vector4)
        {
            vector4.x = Mathf.Max(vector4.x, 0);
            vector4.y = Mathf.Max(vector4.y, 0);
            vector4.z = Mathf.Max(vector4.z, 0);
            vector4.w = Mathf.Max(vector4.w, 0);
            return vector4;
        }

        /// <summary>
        /// Finds the minimum value from multiple float values without allocating memory.
        /// </summary>
        [UsedImplicitly]
        public static float MinOf(float value1, float value2, float value3, float value4, float value5)
            => Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(value1, value2), value3), value4), value5);

        /// <summary>
        /// Calculates the scale factor for Vector4 values that need to fit within rectangular bounds.
        /// Uses a constraint pattern where opposing pairs must fit within width/height.
        /// </summary>
        /// <param name="rect">The rectangle bounds</param>
        /// <param name="values">Vector4 values to scale (x,y pair and z,w pair for width; x,w pair and z,y pair for height)</param>
        /// <returns>Scale factor to apply (clamped to maximum of 1.0)</returns>
        [UsedImplicitly]
        public static float CalculateScaleFactorForBounds(this Rect rect, Vector4 values) =>
            MinOf(
                rect.width / (values.x + values.y),  // First width constraint
                rect.width / (values.z + values.w),  // Second width constraint
                rect.height / (values.x + values.w), // First height constraint
                rect.height / (values.z + values.y), // Second height constraint
                1f                                   // Maximum scale
            );
    }
}