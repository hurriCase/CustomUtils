using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.Base;
using CustomUtils.Runtime.UI.Theme.ThemeMapping;
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
        [field: SerializeField] internal SerializableReactiveProperty<ColorData> ColorData { get; set; } = new();

        private ColorModifierBase _currentColorModifier;

        private ColorType _currentColorType;

        private void OnEnable()
        {
            ColorData.Skip(1).SubscribeUntilDisable(this, (colorData, self) => self.UpdateModifier(colorData));
        }

        private void UpdateModifier(ColorData colorData)
        {
            if (_currentColorType != colorData.ColorType || !_currentColorModifier)
            {
                CreateModifier(colorData.ColorType);
                _currentColorType = colorData.ColorType;
            }

            if (_currentColorModifier)
                _currentColorModifier.UpdateColor(colorData.ColorName);
        }

        private void CreateModifier(ColorType colorType)
        {
            _currentColorModifier.AsNullable()?.Destroy();
            _currentColorModifier = ColorModifierFactory.CreateModifier(colorType, gameObject);
        }
    }
}