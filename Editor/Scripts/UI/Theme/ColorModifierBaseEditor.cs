using CustomUtils.Runtime.UI.Theme.ColorModifiers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.UI.Theme
{
    internal abstract class ColorModifierBaseEditor<TVisualElement, TValueType> : UnityEditor.Editor
        where TVisualElement : BaseField<TValueType>, new()
    {
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private VisualTreeAsset _visualTreeAsset;

        private const string ColorNameDropdownName = "color-name-dropdown";
        protected abstract string ColorPreviewName { get; }

        private ColorModifierBase _colorModifierBase;

        private TVisualElement _colorPreview;
        private DropdownField _colorDropdown;

        public override VisualElement CreateInspectorGUI()
        {
            var container = _visualTreeAsset.CloneTree();

            _colorModifierBase = target as ColorModifierBase;

            if (!_colorModifierBase)
                return container;

            UpdateDropdown(container);

            return container;
        }

        private void UpdateDropdown(VisualElement container)
        {
            _colorDropdown = container.Q<DropdownField>(ColorNameDropdownName);

            _colorDropdown.choices = _colorModifierBase.GetColorNames();
            var colorNameProperty = serializedObject.FindProperty(ColorModifierBase.GetColorNameProperty);
            _colorDropdown.BindProperty(colorNameProperty);

            _colorPreview = container.Q<TVisualElement>(ColorPreviewName);

            container.TrackPropertyValue(colorNameProperty, ChangeColor);
        }

        private void ChangeColor(SerializedProperty colorNameProperty)
        {
            var colorName = colorNameProperty.stringValue;
            _colorPreview.value = GetColor(colorName);

            _colorModifierBase.ApplyColor();
        }

        protected abstract TValueType GetColor(string colorName);
    }
}