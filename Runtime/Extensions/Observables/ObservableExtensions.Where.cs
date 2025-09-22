using System;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions.Observables
{
    /// <summary>
    /// Extension methods for Observable filtering with MonoBehaviour context.
    /// </summary>
    public static partial class ObservableExtensions
    {
        /// <summary>
        /// Filters observable based on predicate that only uses the MonoBehaviour instance.
        /// </summary>
        /// <typeparam name="TSelf">MonoBehaviour type.</typeparam>
        /// <typeparam name="T">Observable value type.</typeparam>
        /// <param name="observable">Observable to filter.</param>
        /// <param name="self">MonoBehaviour instance for predicate.</param>
        /// <param name="predicate">Predicate function that takes only MonoBehaviour instance.</param>
        /// <returns>Filtered observable.</returns>
        [UsedImplicitly]
        public static Observable<T> Where<TSelf, T>(
            this Observable<T> observable,
            TSelf self,
            Func<TSelf, bool> predicate)
            where TSelf : MonoBehaviour
        {
            return observable.Where((self, predicate), static (_, tuple) => tuple.predicate(tuple.self));
        }
    }
}