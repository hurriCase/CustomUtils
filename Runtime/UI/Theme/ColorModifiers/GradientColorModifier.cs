using System.Collections.Generic;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.Databases;
using CustomUtils.Runtime.UI.Theme.VertexGradient;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    internal sealed class GradientColorModifier : ColorModifierBase
    {
        [SerializeField] private SerializableReactiveProperty<GradientDirection> _gradientDirection
            = new(GradientDirection.LeftToRight);

        private void OnEnable()
        {
            _gradientDirection.SubscribeUntilDisable(this, static self => self.ApplyColor());
        }

        private void OnDestroy()
        {
            Graphic.ClearVertexGradient();
        }

        internal override void ApplyColor()
        {
            if (GradientColorDatabase.Instance.TryGetColorByName(ColorName, out var gradient) is false)
                return;

            Graphic.ApplyVertexGradient(gradient, _gradientDirection.Value);
        }

        internal override List<string> GetColorNames() => GradientColorDatabase.Instance.GetColorNames();
    }
}