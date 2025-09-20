using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.UI.Theme.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    public abstract class ThemeStateMappingGeneric<TEnum> : ScriptableObject where TEnum : unmanaged, Enum
    {
        [field: SerializeField] public EnumArray<TEnum, ColorMapping> StateMappings { get; private set; }

        [UsedImplicitly]
        public Color GetColorForState(TEnum state)
        {
            var mapping = StateMappings[state];
            if (mapping.ColorType != ColorType.Solid)
                return default;

            return SolidColorDatabase.Instance.TryGetColorByName(mapping.ColorName, out var color) ? color : default;
        }

        [UsedImplicitly]
        public Gradient GetGradientForState(TEnum state)
        {
            var mapping = StateMappings[state];
            if (mapping.ColorType != ColorType.Gradient)
                return null;

            return GradientColorDatabase.Instance.TryGetColorByName(mapping.ColorName, out var gradient)
                ? gradient
                : null;
        }

        [UsedImplicitly]
        public void SetComponentForState(TEnum state, ThemeComponent themeComponent)
        {
            var mapping = StateMappings[state];

            themeComponent.CurrentColorType.Value = mapping.ColorType;
            themeComponent.UpdateName(mapping.ColorType, mapping.ColorName);
            themeComponent.ApplyColor();
        }
    }
}