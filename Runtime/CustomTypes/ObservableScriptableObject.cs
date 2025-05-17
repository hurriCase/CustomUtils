using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for ScriptableObjects that can notify subscribers when their values change.
    /// </summary>
    /// <typeparam name="T">The type of ScriptableObject being observed.</typeparam>
    public abstract class ObservableScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        /// <summary>
        /// Event that is triggered when a value in the ScriptableObject changes.
        /// </summary>
        /// <remarks>
        /// Subscribers receive the changed ScriptableObject instance as a parameter.
        /// </remarks>
        [UsedImplicitly] public event Action<T> OnValueChanged;

        /// <summary>
        /// Notifies subscribers that a value has changed.
        /// </summary>
        /// <param name="value">The updated ScriptableObject instance.</param>
        [UsedImplicitly] protected void NotifyValueChanged(T value) => OnValueChanged?.Invoke(value);
    }
}