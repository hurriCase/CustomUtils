using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.KeyDrawer
{
    [CustomPropertyDrawer(typeof(LocalizationKey))]
    internal sealed class LocalizationKeyDrawer : PropertyDrawer
    {
        [SerializeField] private StyleSheet _unityFieldStyle;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var guidProperty = property.FindFieldRelative(nameof(LocalizationKey.GUID));
            var rootVisualElement = new LocalizationKeyElement(guidProperty, preferredLabel);

            rootVisualElement.styleSheets.Add(_unityFieldStyle);

            return rootVisualElement;
        }
    }
}