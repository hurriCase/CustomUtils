using System.Collections.Generic;
using JetBrains.Annotations;
using ZLinq;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    [UsedImplicitly]
    public static class ReadOnlyListExtensions
    {
        /// <summary>
        /// Finds the range of indices containing non-zero values in the list.
        /// </summary>
        /// <param name="data">The list of integers to analyze.</param>
        /// <returns>A tuple containing the start and end indices of the non-zero range, or null if all values are zero.</returns>
        [UsedImplicitly]
        public static (int startIndex, int endIndex)? GetNonZeroRange(this IReadOnlyList<int> data)
        {
            using var nonZeroIndices = data
                .AsValueEnumerable()
                .Select((value, index) => (value, index))
                .Where(tuple => tuple.value > 0)
                .Select(tuple => tuple.index)
                .ToArrayPool();

            if (nonZeroIndices.Size == 0)
                return null;

            var zeroIndicesSpan = nonZeroIndices.Span;
            return (zeroIndicesSpan[0], zeroIndicesSpan[^1]);
        }
    }
}