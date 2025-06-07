using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for Unity Component objects.
    /// </summary>
    [UsedImplicitly]
    public static class ComponentExtensions
    {
        /// <summary>
        /// Attempts to find a component of type T in the parent objects of the specified component.
        /// </summary>
        /// <typeparam name="T">The type of component to search for, must inherit from Component.</typeparam>
        /// <param name="component">The component to start the search from.</param>
        /// <param name="result">When this method returns, contains the component of type T if found; otherwise, null.</param>
        /// <returns>
        /// <c>true</c> if a component of type T was found in the parent hierarchy; otherwise, <c>false</c>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
        {
            result = component.GetComponentInParent<T>();
            return result;
        }
    }
}