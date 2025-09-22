using CustomUtils.Runtime.UI.Theme.ColorModifiers;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.UI.Theme
{
    [CustomEditor(typeof(GradientColorModifier))]
    internal sealed class GradientColorModifierEditor : ColorModifierBaseEditor<GradientField, Gradient>
    {
        protected override string ColorPreviewName => "gradient-color-preview";

        protected override Gradient GetColor(string colorName)
        {
            GradientColorDatabase.Instance.TryGetColorByName(ref colorName, out var gradient);
            return gradient;
        }
    }
}