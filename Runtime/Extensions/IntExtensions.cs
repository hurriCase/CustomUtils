using JetBrains.Annotations;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for integer values.
    /// </summary>
    [UsedImplicitly]
    public static class IntExtensions
    {
        /// <summary>
        /// Determines whether the specified integer is a power of two.
        /// </summary>
        /// <param name="x">The integer value to check.</param>
        /// <returns>
        /// <c>true</c> if the specified integer is a power of two (2, 4, 8, 16, etc.);
        /// <c>false</c> if the integer is zero or not a power of two.
        /// </returns>
        [UsedImplicitly]
        public static bool IsPowerOfTwo(this int x) => x != 0 && (x & (x - 1)) == 0;
    }
}