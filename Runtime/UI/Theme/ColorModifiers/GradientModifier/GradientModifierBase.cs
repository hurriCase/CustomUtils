using System.Collections.Generic;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.Databases;
using CustomUtils.Runtime.UI.Theme.ImageVertexGradient;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier
{
    internal abstract class GradientModifierBase : ColorModifierBase
    {
        [field: SerializeField]
        internal SerializableReactiveProperty<GradientDirection> CurrentGradientDirection { get; private set; } =
            new(GradientDirection.LeftToRight);

        protected virtual void OnEnable()
        {
            CurrentGradientDirection.SubscribeUntilDisable(this, static self => self.ApplyColor());
        }

        private void OnDestroy()
        {
            Graphic.ClearVertexGradient();
        }

        internal override void ApplyColor()
        {
            if (GradientColorDatabase.Instance.TryGetColorByName(ref colorName, out var gradient) is false)
                return;

            OnApplyColor(gradient);
        }

        protected abstract void OnApplyColor(Gradient gradient);

        internal override List<string> GetColorNames() => GradientColorDatabase.Instance.GetColorNames();
    }
}