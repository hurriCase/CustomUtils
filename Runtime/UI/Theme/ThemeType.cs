using JetBrains.Annotations;

namespace CustomUtils.Runtime.UI.Theme
{
    /// <summary>
    /// Defines the available theme types for the UI theme system.
    /// </summary>
    [UsedImplicitly]
    public enum ThemeType
    {
        /// <summary>
        /// No theme selected or default state.
        /// </summary>
        [UsedImplicitly]
        None = 0,

        /// <summary>
        /// Light theme variant with bright colors and light backgrounds.
        /// </summary>
        [UsedImplicitly]
        Light = 1,

        /// <summary>
        /// Dark theme variant with dark colors and dark backgrounds.
        /// </summary>
        [UsedImplicitly]
        Dark = 2
    }
}