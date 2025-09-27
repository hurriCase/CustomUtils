using System.Reflection;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime;
using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    internal sealed class ShowIfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not ShowIfAttribute showIfAttribute)
                return;

            var validationResult = ValidateConditionalField(showIfAttribute, property);

            if (validationResult.IsValid is false)
            {
                EditorVisualControls.WarningBox(position, validationResult.ErrorMessage);
                return;
            }

            if (ShouldShowProperty(showIfAttribute, property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is not ShowIfAttribute showIfAttribute)
                return EditorGUI.GetPropertyHeight(property, label);

            var validationResult = ValidateConditionalField(showIfAttribute, property);

            if (validationResult.IsValid is false)
                return EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;

            if (ShouldShowProperty(showIfAttribute, property) is false)
                return -EditorGUIUtility.standardVerticalSpacing;

            return EditorGUI.GetPropertyHeight(property, label);
        }

        private bool ShouldShowProperty(ShowIfAttribute showIfAttribute, SerializedProperty property)
        {
            if (TryGetSourceProperty(showIfAttribute, property, out var sourceProperty) is false)
                return false;

            return showIfAttribute.ShowType switch
            {
                ShowType.True => sourceProperty.boolValue,
                ShowType.False => sourceProperty.boolValue is false,
                _ => true
            };
        }

        private Result ValidateConditionalField(ShowIfAttribute showIfAttribute, SerializedProperty property)
        {
            if (TryGetSourceProperty(showIfAttribute, property, out var sourceProperty))
            {
                if (sourceProperty.propertyType != SerializedPropertyType.Boolean)
                    return Result.Invalid(
                        $"ShowIf field '{showIfAttribute.ConditionalSourceField}' must be a boolean, " +
                        $"but found {sourceProperty.propertyType}");

                return Result.Valid();
            }

            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();

            var info = targetType.GetField(showIfAttribute.ConditionalSourceField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var propertyInfo = targetType.GetProperty(showIfAttribute.ConditionalSourceField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (info == null)
                return Result.Invalid(propertyInfo != null
                    ? $"ShowIf references property '{showIfAttribute.ConditionalSourceField}' " +
                      "but properties are not serialized. Use a [SerializeField] field instead."
                    : $"ShowIf field '{showIfAttribute.ConditionalSourceField}' not found in class {targetType.Name}.");

            var existError = $"ShowIf field '{showIfAttribute.ConditionalSourceField}' exists but is not";
            return Result.Invalid(info.FieldType == typeof(bool)
                ? existError + " serialized. Add [SerializeField] to make it work."
                : existError + $" a boolean ({info.FieldType.Name}).");
        }

        private bool TryGetSourceProperty(
            ShowIfAttribute showIfAttribute,
            SerializedProperty property,
            out SerializedProperty sourceProperty)
        {
            var serializedObject = property.serializedObject;
            var sourceFieldName = showIfAttribute.ConditionalSourceField;
            sourceProperty = serializedObject.FindProperty(sourceFieldName)
                             ?? serializedObject.FindField(sourceFieldName);

            return sourceProperty != null;
        }
    }
}