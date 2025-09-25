﻿using JetBrains.Annotations;

namespace CustomUtils.Runtime.UI.Theme
{
    /// <summary>
    /// Defines the different types of colors that can be applied to UI elements in the theme system.
    /// </summary>
    [UsedImplicitly]
    public enum ColorType
    {
        /// <summary>
        /// No color type specified or default state.
        /// </summary>
        [UsedImplicitly]
        None = 0,

        /// <summary>
        /// Solid color that applies a single uniform color to the UI element.
        /// </summary>
        [UsedImplicitly]
        Solid = 1,

        /// <summary>
        /// Gradient color that applies a gradient effect to graphic UI elements.
        /// </summary>
        [UsedImplicitly]
        GraphicGradient = 2,

        /// <summary>
        /// Gradient color specifically designed for text UI elements using TextMeshPro.
        /// </summary>
        [UsedImplicitly]
        TextGradient = 3
    }
}