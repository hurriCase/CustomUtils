using CustomUtils.Runtime.CustomBehaviours;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.VertexGradient;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public sealed class ThemeComponent : GraphicBehaviour
    {
        [field: SerializeField] public SerializableReactiveProperty<ColorType> CurrentColorType { get; set; } = new();

        [field: SerializeField]
        public SerializableReactiveProperty<GradientDirection> CurrentGradientDirection { get; set; } = new();

        [field: SerializeField] public EnumArray<ColorType, string> ColorNames { get; set; } = new(EnumMode.SkipFirst);

        private void Reset()
        {
            ApplyColor();
        }

        private void OnEnable()
        {
            ApplyColor();
        }

        private void Awake()
        {
            ThemeHandler.CurrentThemeType.SubscribeAndRegister(this, self => self.ApplyColor());
            CurrentColorType.SubscribeAndRegister(this, self => self.ApplyColor());
            CurrentGradientDirection.SubscribeAndRegister(this, self => self.ApplyColor());
        }

        [UsedImplicitly]
        public void UpdateName(ColorType colorType, string newName)
        {
            var colorNames = ColorNames;
            colorNames[colorType] = newName;
            ColorNames = colorNames;
        }

        [UsedImplicitly]
        public void ApplyColor()
        {
            if (!Graphic || CurrentColorType.Value == ColorType.None)
                return;

            var colorName = ColorNames[CurrentColorType.Value];

            switch (CurrentColorType.Value)
            {
                case ColorType.Gradient:
                    if (GradientColorDatabase.Instance.TryGetColorByName(colorName, out var gradient))
                        Graphic.ApplyVertexGradient(gradient, CurrentGradientDirection.Value);
                    break;

                case ColorType.Solid:
                    if (SolidColorDatabase.Instance.TryGetColorByName(colorName, out var color) is false)
                        return;

                    Graphic.ClearVertexGradient();
                    Graphic.color = color;
                    break;
            }
        }
    }
}