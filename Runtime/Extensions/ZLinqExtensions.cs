using JetBrains.Annotations;
using ZLinq;

namespace CustomUtils.Runtime.Extensions
{
    internal static class ZLinqExtensions
    {
        /// <summary>
        /// Gets a random element from the ZLinq ValueEnumerable using Unity's Random number generator.
        /// </summary>
        /// <typeparam name="TEnumerator">The type of the enumerator.</typeparam>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="source">The ZLinq ValueEnumerable to get a random element from.</param>
        /// <returns>A randomly selected element from the enumerable.</returns>
        [UsedImplicitly]
        public static T Random<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source)
            where TEnumerator : struct, IValueEnumerator<T>
        {
            var count = source.Count();
            var randomIndex = UnityEngine.Random.Range(0, count);
            return source.ElementAt(randomIndex);
        }
    }
}