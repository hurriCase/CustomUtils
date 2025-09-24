using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Extensions
{
    /// <summary>
    /// Provides Editor time extension methods for <see cref="BaseField{T}"/>.
    /// </summary>
    public static class BaseFieldExtensionsEditor
    {
        private const string ValuePropertyName = "value";

        /// <summary>
        /// Binds a reactive property from a serialized object to a UI element,
        /// enabling two-way binding and change notifications.
        /// </summary>
        /// <typeparam name="T">The type of the property being bound.</typeparam>
        /// <param name="element">The UI element to bind the property to.</param>
        /// <param name="serializedObject">The serialized object containing the property to bind.</param>
        /// <param name="propertyName">The name of the reactive property to bind.</param>
        [UsedImplicitly]
        public static void BindReactiveProperty<T>(
            this BaseField<T> element,
            SerializedObject serializedObject,
            string propertyName)
        {
            var reactivePropertyField = serializedObject.FindField(propertyName);
            var valueProperty = reactivePropertyField.FindPropertyRelative(ValuePropertyName);

            element.BindProperty(valueProperty);

            element.RegisterValueChangedCallback(_ =>
            {
                serializedObject.ApplyModifiedProperties();

                TriggerReactivePropertyNotification(serializedObject, reactivePropertyField);
            });
        }

        private static void TriggerReactivePropertyNotification(
            SerializedObject serializedObject,
            SerializedProperty reactivePropertyField)
        {
            try
            {
                var targetObject = serializedObject.targetObject;
                var reactivePropertyInstance =
                    GetReactivePropertyInstance(targetObject, reactivePropertyField.propertyPath);

                if (reactivePropertyInstance == null)
                    return;

                var forceNotifyMethod = reactivePropertyInstance.GetType().GetMethod("ForceNotify",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                forceNotifyMethod?.Invoke(reactivePropertyInstance, Array.Empty<object>());
            }
            catch (Exception ex)
            {
                Debug.LogError("[NotifyValueChangedExtensionsEditor::TriggerReactivePropertyNotification] '" +
                               $"Failed to trigger reactive property notification: {ex.Message}");

                Debug.LogException(ex);
            }
        }

        private static object GetReactivePropertyInstance(object targetObject, string propertyPath)
        {
            var pathParts = propertyPath.Split('.');
            var currentObject = targetObject;

            foreach (var pathPart in pathParts)
            {
                if (currentObject == null)
                    break;

                var fieldInfo = currentObject.GetType().GetFieldInfo(pathPart);

                if (fieldInfo != null)
                    currentObject = fieldInfo.GetValue(currentObject);
                else
                    return null;
            }

            return currentObject;
        }
    }
}