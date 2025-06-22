﻿using CustomUtils.Runtime.Extensions.GradientExtensions;
using CustomUtils.Runtime.UI.Theme.ThemeColors;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    public interface IBaseThemeComponent
    {
        public void OnApplyColor();
        public ColorType ColorType { get; set; }
        public ThemeSolidColor ThemeSolidColor { get; }
        public ThemeGradientColor ThemeGradientColor { get; }
        public ThemeSharedColor ThemeSharedColor { get; }
        public string ThemeSolidColorName { get; set; }
        public string ThemeGradientColorName { get; set; }
        public string ThemeSharedColorName { get; set; }
        public GradientDirection GradientDirection { get; set; }
    }
}