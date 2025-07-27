using JetBrains.Annotations;

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
        /// <returns>True if the value is greater than the lower threshold and less than the upper threshold; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool IsReasonable(this float value) => value is > LowerThreshold and < UpperThreshold;
    }
}