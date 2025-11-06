using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.Drawer
{
    [CustomPropertyDrawer(typeof(LocalizationKey))]
    internal sealed class LocalizationKeyDrawer : PropertyDrawer
    {
        [SerializeField] private StyleSheet _styleSheet;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var guidProperty = property.FindFieldRelative(nameof(LocalizationKey.GUID));
            var rootVisualElement = new LocalizationKeyElement(property, guidProperty, preferredLabel);

            if (LocalizationRegistry.Instance.Entries.TryGetValue(guidProperty.stringValue, out var selectedEntry))
                rootVisualElement.Initialize(selectedEntry);

            rootVisualElement.styleSheets.Add(_styleSheet);

            return rootVisualElement;
        }
    }
}