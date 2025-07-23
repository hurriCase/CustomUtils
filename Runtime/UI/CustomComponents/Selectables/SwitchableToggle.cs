using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    internal sealed class SwitchableToggle : Toggle
    {
        [field: SerializeField] internal GameObject CheckedObject { get; private set; }
        [field: SerializeField] internal GameObject UncheckedObject { get; private set; }

        protected override void Reset()
        {
            base.Reset();

            transition = Transition.None;
        }

        protected override void Awake()
        {
            base.Awake();

            this.OnValueChangedAsObservable()
                .Subscribe(this, static (isOn, toggle) =>
                {
                    if (!toggle.CheckedObject || !toggle.UncheckedObject)
                        return;

                    toggle.CheckedObject.SetActive(isOn);
                    toggle.UncheckedObject.SetActive(!isOn);
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}