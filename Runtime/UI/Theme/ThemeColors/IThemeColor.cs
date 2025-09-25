using CustomUtils.Runtime.CustomTypes.Collections;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    internal interface IThemeColor<TColor>
    {
        string Name { get; }
        EnumArray<ThemeType, TColor> Colors { get; }
    }
}