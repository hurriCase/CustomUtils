using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKey))]
    internal sealed class LocalizationKeyDrawer : PropertyDrawer
    {
        [SerializeField] private VisualTreeAsset _localizationKeyLayout;

        private SerializedProperty _keyProperty;
        private SerializedProperty _tableNameProperty;
        private SerializedProperty _guidProperty;

        private DropdownField _keyLabel;

        private SerializedProperty _serializedProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _serializedProperty = property;

            var container = _localizationKeyLayout.CloneTree();

            SetProperties();

            _keyLabel = container.Q<DropdownField>("LocalizationKeyLabel");
            _keyLabel.RegisterCallback<MouseDownEvent>(ShowKeySelectionWindow, TrickleDown.TrickleDown);

            UpdateKeyFieldDisplay(_keyProperty.stringValue);

            return container;
        }

        private void SetProperties()
        {
            _keyProperty = _serializedProperty.FindFieldRelative(nameof(LocalizationKey.Key));
            _tableNameProperty = _serializedProperty.FindFieldRelative(nameof(LocalizationKey.TableName));
            _guidProperty = _serializedProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
        }

        private void ShowKeySelectionWindow(MouseDownEvent _)
        {
            var currentKey = new LocalizationKey(
                _guidProperty.stringValue,
                _keyProperty.stringValue,
                _tableNameProperty.stringValue
            );

            LocalizationSelectorWindow.ShowWindow(currentKey, OnSelectionChanged);
        }

        private void OnSelectionChanged(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.Guid;
            _keyProperty.stringValue = selectedEntry.Key;
            _tableNameProperty.stringValue = selectedEntry.TableName;

            _serializedProperty.serializedObject.ApplyModifiedProperties();

            UpdateKeyFieldDisplay(selectedEntry.Key);
        }

        private void UpdateKeyFieldDisplay(string key)
        {
            var displayText = string.IsNullOrEmpty(key) ? "[None]" : key;
            _keyLabel.value = displayText;
        }
    }
}