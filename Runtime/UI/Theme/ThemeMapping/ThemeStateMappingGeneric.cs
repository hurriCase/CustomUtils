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
        public void SetComponentForState(TEnum state, ThemeComponent themeComponent)
        {
            var mapping = StateMappings[state];

            themeComponent.CurrentColorType.Value = mapping.ColorType;
            themeComponent.UpdateColor(mapping.ColorType, mapping.ColorName);
            themeComponent.ApplyColor();
        }
    }
}