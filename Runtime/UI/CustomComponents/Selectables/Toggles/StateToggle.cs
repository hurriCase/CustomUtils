using System.Collections.Generic;
using CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Mappings;
using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles
{
    [UsedImplicitly]
    public class StateToggle : Toggle
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public Image Image { get; private set; }
        [field: SerializeField] public List<GameObject> CheckedObjects { get; private set; }
        [field: SerializeField] public List<GameObject> UncheckedObjects { get; private set; }
        [field: SerializeField] public ToggleGraphicMapping[] AdditionalGraphics { get; private set; }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            transition = Transition.None;
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            this.OnValueChangedAsObservable()
                .Subscribe(this, static (isOn, toggle) => toggle.HandleStateChange(isOn))
                .RegisterTo(destroyCancellationToken);

            ApplyGraphics(currentSelectionState);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            ApplyGraphics(state);
        }

        private void HandleStateChange(bool isOn)
        {
            SwitchObjects(isOn);
            ApplyGraphics(currentSelectionState);
        }

        private void SwitchObjects(bool isOn)
        {
            if (CheckedObjects is not null)
                foreach (var checkedObject in CheckedObjects)
                    checkedObject.SetActive(isOn);

            if (UncheckedObjects is null)
                return;

            foreach (var uncheckedObject in UncheckedObjects)
                uncheckedObject.SetActive(isOn is false);
        }

        private void ApplyGraphics(SelectionState state)
        {
            if (AdditionalGraphics is null || AdditionalGraphics.Length == 0)
                return;

            var mappedState = isOn ? ToggleStateType.On : MapSelectionStateToToggleState(state);

            foreach (var graphicMapping in AdditionalGraphics)
                graphicMapping.ApplyState(mappedState);
        }

        private ToggleStateType MapSelectionStateToToggleState(SelectionState state) =>
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