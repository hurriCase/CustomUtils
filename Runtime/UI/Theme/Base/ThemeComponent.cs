using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.ColorModifiers;
using CustomUtils.Runtime.UI.Theme.ThemeMapping;
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
        [SerializeField] private SerializableReactiveProperty<ColorType> _currentColorType = new();

        [SerializeField, HideInInspector] private ColorModifierBase _currentColorModifier;

        private void OnEnable()
        {
            ThemeHandler.CurrentThemeType.SubscribeUntilDisable(this, self => self.ApplyColor());
            _currentColorType
                .Skip(1)
                .SubscribeUntilDisable(this, self => self.UpdateModifier());
        }

        [UsedImplicitly]
        public void UpdateColor(ColorData colorData)
        {
            _currentColorType.Value = colorData.ColorType;
            _currentColorModifier.UpdateColor(colorData.ColorName);
        }

        [UsedImplicitly]
        public void ApplyColor()
        {
            if (_currentColorType.Value == ColorType.None)
                return;

            _currentColorModifier.ApplyColor();
        }

        private void UpdateModifier()
        {
            _currentColorModifier.AsNullable()?.Destroy();
            _currentColorModifier = ColorModifierFactory.CreateModifier(_currentColorType.Value, gameObject);

            ApplyColor();
        }
    }
}