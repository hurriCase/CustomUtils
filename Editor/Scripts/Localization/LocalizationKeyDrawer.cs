using CustomUtils.Editor.Scripts.Extensions;
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

        private SerializedProperty _serializedProperty;
        private SerializedProperty _guidProperty;
        private SerializedProperty _keyProperty;
        private SerializedProperty _tableNameProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _serializedProperty = property;
            _guidProperty = property.FindFieldRelative(nameof(LocalizationKey.Guid));
            _keyProperty = property.FindFieldRelative(nameof(LocalizationKey.Key));
            _tableNameProperty = property.FindFieldRelative(nameof(LocalizationKey.TableName));

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var hasValidGuid = string.IsNullOrEmpty(_guidProperty.stringValue) is false;
            var isValid = false;

            if (hasValidGuid)
                if (LocalizationRegistry.Instance.TryGetEntry(_guidProperty.stringValue, out var entry))
                {
                    isValid = true;

                    if (_keyProperty.stringValue != entry.Key)
                    {
                        _keyProperty.stringValue = entry.Key;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }

                    if (_tableNameProperty.stringValue != entry.TableName)
                    {
                        _tableNameProperty.stringValue = entry.TableName;
                        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                }

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);

            var iconRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                ValidationIconWidth,
                position.height
            );
            DrawValidationIcon(iconRect, isValid);

            var fieldRect = new Rect(
                iconRect.x + iconRect.width + Spacing,
                position.y,
                position.width - EditorGUIUtility.labelWidth - ValidationIconWidth - ButtonWidth - Spacing * 3,
                position.height
            );
            DrawKeyField(fieldRect, _keyProperty.stringValue, _tableNameProperty.stringValue, isValid);

            var buttonRect = new Rect(
                position.x + position.width - ButtonWidth,
                position.y,
                ButtonWidth,
                position.height
            );

            var currentKey = new LocalizationKey(
                _guidProperty.stringValue,
                _keyProperty.stringValue,
                _tableNameProperty.stringValue
            );

            if (GUI.Button(buttonRect, "Select", EditorStyles.miniButton))
                LocalizationSelectorWindow.ShowWindow(
                    currentKey,
                    SelectEntry);

            EditorGUI.EndProperty();
        }

        private void SelectEntry(LocalizationEntry entry)
        {
            _guidProperty.stringValue = entry.Guid;
            _keyProperty.stringValue = entry.Key;
            _tableNameProperty.stringValue = entry.TableName;

            _serializedProperty.serializedObject.ApplyModifiedProperties();
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