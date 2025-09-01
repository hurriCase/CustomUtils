using CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Mappings;
using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Components
{
    [UsedImplicitly]
    public class ThemeToggle : Toggle
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public ToggleGraphicMapping[] AdditionalGraphics { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            this.OnValueChangedAsObservable()
                .Subscribe(this, static (isOn, toggle) => toggle
                    .DoStateTransition(isOn ? SelectionState.Selected : toggle.currentSelectionState, false))
                .RegisterTo(destroyCancellationToken);

            ApplyGraphics(currentSelectionState);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            ApplyGraphics(state);
        }

        private void ApplyGraphics(SelectionState state)
        {
            if (AdditionalGraphics is null || AdditionalGraphics.Length == 0)
                return;

            var mappedState = isOn ? ToggleStateType.On : MapSelectionStateToSelectableState(state);

            foreach (var graphicMapping in AdditionalGraphics)
                graphicMapping.ApplyState(mappedState, transition);
        }

        private ToggleStateType MapSelectionStateToSelectableState(SelectionState state) =>
            state switch
            {
                SelectionState.Normal => ToggleStateType.Normal,
                SelectionState.Highlighted => ToggleStateType.Highlighted,
                SelectionState.Pressed => ToggleStateType.Pressed,
                SelectionState.Selected => ToggleStateType.Selected,
                SelectionState.Disabled => ToggleStateType.Disabled,
                _ => ToggleStateType.Normal
            };
    }
}