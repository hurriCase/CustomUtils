using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKey))]
    internal sealed class LocalizationKeyDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 60f;
        private const float ValidationIconWidth = 20f;
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var guidProperty = property.FindPropertyRelative("_guid");
            var keyProperty = property.FindPropertyRelative("_key");
            var tableProperty = property.FindPropertyRelative("_tableName");

            // Validate and sync the key with registry
            var hasValidGuid = string.IsNullOrEmpty(guidProperty.stringValue) is false;
            var isValid = false;

            if (hasValidGuid)
            {
                if (LocalizationRegistry.Instance.TryGetEntry(guidProperty.stringValue, out var entry))
                {
                    isValid = true;

                    // Fix visual bug: Update key if it changed in the registry
                    if (keyProperty.stringValue != entry.Key)
                    {
                        keyProperty.stringValue = entry.Key;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }

                    if (tableProperty.stringValue != entry.TableName)
                    {
                        tableProperty.stringValue = entry.TableName;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                }
            }

            // Draw label
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            // Draw validation icon
            var iconRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                ValidationIconWidth,
                position.height
            );
            DrawValidationIcon(iconRect, isValid);

            // Draw key display field (read-only)
            var fieldRect = new Rect(
                iconRect.x + iconRect.width + Spacing,
                position.y,
                position.width - EditorGUIUtility.labelWidth - ValidationIconWidth - ButtonWidth - Spacing * 3,
                position.height
            );
            DrawKeyField(fieldRect, keyProperty.stringValue, tableProperty.stringValue, isValid);

            // Draw select button
            var buttonRect = new Rect(
                position.x + position.width - ButtonWidth,
                position.y,
                ButtonWidth,
                position.height
            );

            if (GUI.Button(buttonRect, "Select", EditorStyles.miniButton))
            {
                LocalizationKeySelectorWindow.Show(property, () =>
                {
                    property.serializedObject.Update();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                });
            }

            EditorGUI.EndProperty();
        }

        private void DrawValidationIcon(Rect position, bool isValid)
        {
            var icon = isValid ? "✔" : "✘";
            var color = isValid ? Color.green : Color.red;

            var previousColor = GUI.color;
            GUI.color = color;

            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUI.LabelField(position, icon, style);
            GUI.color = previousColor;
        }

        private void DrawKeyField(Rect position, string key, string tableName, bool isValid)
        {
            var displayText = string.IsNullOrEmpty(key)
                ? "[None]"
                : string.IsNullOrEmpty(tableName)
                    ? key
                    : $"[{tableName}] {key}";

            var previousColor = GUI.color;
            GUI.color = isValid ? Color.white : new Color(0.7f, 0.7f, 0.7f);

            var style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            EditorGUI.LabelField(position, displayText, style);
            GUI.color = previousColor;
        }
    }
}