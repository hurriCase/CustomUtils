using CustomUtils.Runtime.UI.CustomComponents.Selectables.Base;
using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [UsedImplicitly]
    public class ThemeToggle : Toggle
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public SelectableColorMapping SelectableColorMapping { get; private set; }
        [field: SerializeField] public ThemeGraphicMapping[] AdditionalGraphics { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            this.OnValueChangedAsObservable()
                .Subscribe(this, static (isOn, toggle) => toggle
                    .DoStateTransition(isOn ? SelectionState.Selected : toggle.currentSelectionState, false))
                .RegisterTo(destroyCancellationToken);

            ApplyTheme();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (isOn && state != SelectionState.Disabled)
            {
                base.DoStateTransition(SelectionState.Selected, instant);

                ApplyAdditionalGraphics(SelectionState.Selected);
            }
            else
            {
                base.DoStateTransition(state, instant);

                ApplyAdditionalGraphics(state);
            }
        }

        private void ApplyTheme()
        {
            if (!SelectableColorMapping || transition != Transition.ColorTint)
                return;

            colors = SelectableColorMapping.GetThemeBlockColors(colors);
        }

        private void ApplyAdditionalGraphics(SelectionState state)
        {
            if (AdditionalGraphics is null || AdditionalGraphics.Length == 0)
                return;

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