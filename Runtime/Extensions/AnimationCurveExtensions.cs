using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Gets the value of the animation curve at its last keyframe's time.
        /// This is useful for determining the final/maximum value that the curve reaches.
        /// </summary>
        /// <param name="animationCurve">The animation curve to evaluate.</param>
        /// <returns>The curve value at the last keyframe's time position.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if the curve has no keyframes.</exception>
        [UsedImplicitly]
        public static float GetLastValue(this AnimationCurve animationCurve)
            => animationCurve.Evaluate(animationCurve.keys[^1].time);
    }
}