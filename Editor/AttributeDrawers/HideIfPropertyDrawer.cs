using System.Reflection;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not HideIfAttribute hideIfAttribute)
                return;

            var validationResult = ValidateConditionalField(hideIfAttribute, property);

            if (validationResult.IsValid is false)
            {
                EditorVisualControls.WarningBox(position, validationResult.ErrorMessage);
                return;
            }

            if (GetHideIfAttributeResult(hideIfAttribute, property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is not HideIfAttribute hideIfAttribute)
                return EditorGUI.GetPropertyHeight(property, label);

            var validationResult = ValidateConditionalField(hideIfAttribute, property);

            if (validationResult.IsValid is false)
                return EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;

            if (GetHideIfAttributeResult(hideIfAttribute, property))
                return EditorGUI.GetPropertyHeight(property, label);

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool GetHideIfAttributeResult(HideIfAttribute hideIfAttribute, SerializedProperty property)
        {
            var sourcePropertyValue = property.serializedObject.FindProperty(hideIfAttribute.ConditionalSourceField);

            if (sourcePropertyValue != null)
                return sourcePropertyValue.boolValue == hideIfAttribute.HideIfSourceTrue;

            return true;
        }

        private Result ValidateConditionalField(HideIfAttribute hideIfAttribute, SerializedProperty property)
        {
            var sourceProperty = property.serializedObject.FindProperty(hideIfAttribute.ConditionalSourceField);

            if (sourceProperty != null)
            {
                if (sourceProperty.propertyType != SerializedPropertyType.Boolean)
                    return Result.Invalid(
                        $"HideIf field '{hideIfAttribute.ConditionalSourceField}' must be a boolean, " +
                        $"but found {sourceProperty.propertyType}");

                return Result.Valid();
            }

            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();

            var info = targetType.GetField(hideIfAttribute.ConditionalSourceField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var propertyInfo = targetType.GetProperty(hideIfAttribute.ConditionalSourceField,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (info == null)
                return Result.Invalid(propertyInfo != null
                    ? $"HideIf references property '{hideIfAttribute.ConditionalSourceField}' " +
                      "but properties are not serialized. Use a [SerializeField] field instead."
                    : $"HideIf field '{hideIfAttribute.ConditionalSourceField}' not found in class {targetType.Name}.");

            var existError = $"HideIf field '{hideIfAttribute.ConditionalSourceField}' exists but is not";
            return Result.Invalid(info.FieldType == typeof(bool)
                ? existError + "serialized. Add [SerializeField] to make it work."
                : existError + " a boolean ({info.FieldType.Name}).");
        }
    }
}