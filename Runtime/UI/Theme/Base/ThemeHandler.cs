using JetBrains.Annotations;
using R3;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    public static class ThemeHandler
    {
        [UsedImplicitly]
        public static ReactiveProperty<ThemeType> CurrentThemeType { get; } = new(ThemeType.Light);
    }
}