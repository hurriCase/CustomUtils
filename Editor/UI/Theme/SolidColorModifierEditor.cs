using CustomUtils.Runtime.UI.Theme.ColorModifiers;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace CustomUtils.Editor.UI.Theme
{
    [CustomEditor(typeof(SolidColorModifier))]
    internal sealed class SolidColorModifierEditor : ColorModifierBaseEditor<ColorField, Color>
    {
        protected override string ColorPreviewName => "solid-color-preview";

        protected override Color GetColor(string colorName)
            => SolidColorDatabase.Instance.TryGetColorByName(colorName, out var color) ? color : Color.white;
    }
}