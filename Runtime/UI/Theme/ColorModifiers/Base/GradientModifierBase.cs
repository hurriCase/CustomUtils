using System;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.GradientHelpers.Base;
using CustomUtils.Runtime.UI.Theme.Databases;
using CustomUtils.Runtime.UI.Theme.Databases.Base;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    [Serializable]
    internal abstract class GradientModifierBase<TGradientEffect, TComponent> : GenericColorModifierBase<Gradient>
        where TGradientEffect : GradientEffectBase<TComponent>, new()
        where TComponent : Component
    {
        [field: SerializeField]
        internal SerializableReactiveProperty<GradientDirection> CurrentGradientDirection { get; private set; } =
            new(GradientDirection.LeftToRight);

        internal override IThemeDatabase<Gradient> ThemeDatabase => GradientColorDatabase.Instance;

        protected TGradientEffect gradientEffectBase;

        [SerializeField, InspectorReadOnly] private TComponent _component;

        protected override void OnEnable()
        {
            base.OnEnable();

            gradientEffectBase = gradientEffectBase ?? new TGradientEffect();

            _component = _component.AsNullable() ?? GetComponent<TComponent>();

            CurrentGradientDirection.SubscribeUntilDisable(this, static self => self.UpdateColor(self.currentColorName));
        }

        protected override void OnApplyColor(Gradient gradient)
        {
            gradientEffectBase.ApplyGradient(_component, gradient, CurrentGradientDirection.Value);
        }

        private void OnDestroy()
        {
            gradientEffectBase.ClearGradient(_component);
        }
    }
}