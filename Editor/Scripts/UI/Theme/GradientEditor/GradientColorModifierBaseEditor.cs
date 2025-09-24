using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.UI.Theme.GradientEditor
{
    internal abstract class GradientColorModifierEditorBase : ColorModifierBaseEditor<GradientField, Gradient>
    {
        protected override string ColorPreviewName => "gradient-color-preview";
        private const string GradientDirectionName = "gradient-direction-dropdown";

        protected override void OnCreateInspectorGUI(VisualElement container)
        {
            var directionDropdown = container.Q<EnumField>(GradientDirectionName);
            directionDropdown.BindReactiveProperty(serializedObject, nameof(GradientModifierBase.CurrentGradientDirection));
        }

        protected override Gradient GetColor(string colorName)
        {
            GradientColorDatabase.Instance.TryGetColorByName(ref colorName, out var gradient);
            return gradient;
        }
    }
}