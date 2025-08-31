using System.Collections.Generic;
using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Components
{
    [UsedImplicitly]
    public class SwitchableToggle : Toggle
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
        [field: SerializeField] public List<GameObject> CheckedObjects { get; private set; }
        [field: SerializeField] public List<GameObject> UncheckedObjects { get; private set; }

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
                .Subscribe(this, static (isOn, toggle) => toggle.SwitchState(isOn))
                .RegisterTo(destroyCancellationToken);
        }

        private void SwitchState(bool isOn)
        {
            foreach (var checkedObject in CheckedObjects)
                checkedObject.SetActive(isOn);

            foreach (var checkedObject in UncheckedObjects)
                checkedObject.SetActive(isOn is false);
        }
    }
}