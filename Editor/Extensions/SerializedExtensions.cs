using System;
using CustomUtils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Editor.Extensions
{
    public static class SerializedExtensions
    {
        /// <summary>
        /// Finds a serialized property using its backing field name format.
        /// </summary>
        /// <param name="serializedObject">The serialized object to search in.</param>
        /// <param name="name">The property name to find (will be converted to backing field format).</param>
        /// <returns>The found serialized property or null if not found.</returns>
        [UsedImplicitly]
        public static SerializedProperty FindField(this SerializedObject serializedObject, string name) =>
            serializedObject.FindProperty(name.ConvertToBackingField());

        /// <summary>
        /// Finds a relative serialized property using its backing field name format.
        /// </summary>
        /// <param name="serializedProperty">The serialized property to search in.</param>
        /// <param name="name">The property name to find (will be converted to backing field format).</param>
        /// <returns>The found relative serialized property or null if not found.</returns>
        [UsedImplicitly]
        public static SerializedProperty FindFieldRelative(this SerializedProperty serializedProperty, string name) =>
            serializedProperty.FindPropertyRelative(name.ConvertToBackingField());

        /// <summary>
        /// Tries to get a component from the target object.
        /// </summary>
        /// <param name="serializedProperty">The serialized property to get the component from.</param>
        /// <param name="requestedComponent">The requested component.</param>
        /// <returns>True if the component was found; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryGetComponent<TComponent>(
            this SerializedProperty serializedProperty,
            out TComponent requestedComponent)
            where TComponent : Component
        {
            requestedComponent = null;

            return serializedProperty.serializedObject.targetObject is Component component
                   && component.TryGetComponent(out requestedComponent);
        }

        /// <summary>
        /// Attempts to retrieve a component of the specified type from the target object of the serialized property.
        /// </summary>
        /// <param name="serializedProperty">The serialized property associated with the object to inspect.</param>
        /// <param name="componentType">The type of the component to retrieve.</param>
        /// <param name="requestedComponent">When this method returns, contains the component if found, otherwise null.</param>
        /// <returns>True if the component of the specified type is found; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryGetComponent(
            this SerializedProperty serializedProperty,
            Type componentType,
            out Component requestedComponent)
        {
            requestedComponent = null;

            return serializedProperty.serializedObject.targetObject is Component component
                   && component.TryGetComponent(componentType, out requestedComponent);
        }

        [UsedImplicitly]
        public static bool TryGetComponent<TComponent>(
            this SerializedObject serializedProperty,
            out TComponent requestedComponent)
            where TComponent : Component
        {
            requestedComponent = null;

            return serializedProperty.targetObject is Component component
                   && component.TryGetComponent(out requestedComponent);
        }
    }
}