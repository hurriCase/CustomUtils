using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.UI.Theme.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    public abstract class ThemeStateMappingGeneric<TEnum> : ScriptableObject where TEnum : unmanaged, Enum
    {
        [field: SerializeField] public EnumArray<TEnum, ColorData> StateMappings { get; private set; }

        [UsedImplicitly]
        public void SetComponentForState(TEnum state, ThemeComponent themeComponent)
        {
            themeComponent.ColorData.Value = StateMappings[state];
        }
    }
}