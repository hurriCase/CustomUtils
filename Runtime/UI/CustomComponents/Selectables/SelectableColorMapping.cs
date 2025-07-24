using CustomUtils.Runtime.UI.Theme.ThemeMapping;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [UsedImplicitly]
    [CreateAssetMenu(fileName = nameof(SelectableColorMapping), menuName = nameof(SelectableColorMapping))]
    public sealed class SelectableColorMapping : ThemeStateMappingGeneric<SelectableStateType>
    {
        [UsedImplicitly]
        public ColorBlock GetThemeBlockColors(ColorBlock colorBlock)
        {
            colorBlock.normalColor = GetColorForState(SelectableStateType.Normal);
            colorBlock.highlightedColor = GetColorForState(SelectableStateType.Highlighted);
            colorBlock.pressedColor = GetColorForState(SelectableStateType.Pressed);
            colorBlock.selectedColor = GetColorForState(SelectableStateType.Selected);
            colorBlock.disabledColor = GetColorForState(SelectableStateType.Disabled);

            return colorBlock;
        }
    }
}