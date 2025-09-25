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
    internal abstract class GradientModifierBase<TGradientEffect, TComponent> : GenericColorModifierBase<Gradient>
        where TGradientEffect : GradientEffectBase<TComponent>, new()
        where TComponent : Component
    {
        [field: SerializeField]
        internal SerializableReactiveProperty<GradientDirection> CurrentGradientDirection { get; private set; } =
            new(GradientDirection.LeftToRight);

        [SerializeField, InspectorReadOnly] private TComponent _component;

        protected override IThemeDatabase<Gradient> ThemeDatabase => GradientColorDatabase.Instance;

        private TGradientEffect _gradientEffectBase;

        protected override void Awake()
        {
            base.Awake();

            _gradientEffectBase = _gradientEffectBase ?? new TGradientEffect();

            _component = _component.AsNullable() ?? GetComponent<TComponent>();

            CurrentGradientDirection.SubscribeUntilDestroy(this, static self => self.UpdateColor(self.currentColorName));
        }

        protected override void OnApplyColor(Gradient gradient)
        {
            _gradientEffectBase.ApplyGradient(_component, gradient, CurrentGradientDirection.Value);
        }

        public override void Dispose()
        {
            _gradientEffectBase.ClearGradient(_component);
        }
    }
}