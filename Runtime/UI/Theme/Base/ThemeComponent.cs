using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.ColorModifiers;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public sealed class ThemeComponent : MonoBehaviour
    {
        [field: SerializeField] public SerializableReactiveProperty<ColorType> CurrentColorType { get; set; } = new();

        [SerializeField, HideInInspector] private ColorModifierBase _currentColorModifier;

        private void OnEnable()
        {
            ThemeHandler.CurrentThemeType.SubscribeUntilDisable(this, self => self.ApplyColor());
            CurrentColorType.SubscribeUntilDisable(this, self => self.UpdateModifier());
        }

        [UsedImplicitly]
        public void UpdateColor(ColorType colorType, string newName)
        {
            CurrentColorType.Value = colorType;
            _currentColorModifier.UpdateColor(newName);
        }

        [UsedImplicitly]
        public void ApplyColor()
        {
            if (CurrentColorType.Value == ColorType.None)
                return;

            _currentColorModifier.ApplyColor();
        }

        private void UpdateModifier()
        {
            _currentColorModifier.AsNullable()?.Destroy();
            _currentColorModifier = null;

            switch (CurrentColorType.Value)
            {
                case ColorType.Solid:
                    _currentColorModifier = this.GetOrAddComponent<SolidColorModifier>();
                    break;

                case ColorType.Gradient:
                    _currentColorModifier = this.GetOrAddComponent<GradientColorModifier>();
                    break;
            }

            ApplyColor();
        }
    }
}