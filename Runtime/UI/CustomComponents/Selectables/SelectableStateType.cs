﻿using JetBrains.Annotations;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [UsedImplicitly]
    public enum SelectableStateType
    {
        None = 0,
        Normal = 1,
        Highlighted = 2,
        Pressed = 3,
        Selected = 4,
        Disabled = 5
    }
}