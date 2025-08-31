using CustomUtils.Runtime.UI.CustomComponents.Selectables.Base;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [UsedImplicitly]
    public class ThemeButton : Button
    {
        [field: SerializeField] public TextMeshProUGUI TextMeshProUGUI { get; private set; }
        [field: SerializeField] public SelectableColorMapping SelectableColorMapping { get; private set; }
        [field: SerializeField] public ThemeGraphicMapping[] AdditionalGraphics { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (!SelectableColorMapping || transition != Transition.ColorTint)
                return;

            colors = SelectableColorMapping.GetThemeBlockColors(colors);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            if (AdditionalGraphics is null || AdditionalGraphics.Length == 0)
                return;

            ApplyAdditionalGraphics(state);
        }

        private void ApplyAdditionalGraphics(SelectionState state)
        {
            var mappedState = MapSelectionStateToSelectableState(state);

            foreach (var graphicMapping in AdditionalGraphics)
                graphicMapping.ApplyColor(mappedState);
        }

        private SelectableStateType MapSelectionStateToSelectableState(SelectionState state) =>
            state switch
            {
                SelectionState.Normal => SelectableStateType.Normal,
                SelectionState.Highlighted => SelectableStateType.Highlighted,
                SelectionState.Pressed => SelectableStateType.Pressed,
                SelectionState.Selected => SelectableStateType.Selected,
                SelectionState.Disabled => SelectableStateType.Disabled,
                _ => SelectableStateType.Normal
            };
    }
}