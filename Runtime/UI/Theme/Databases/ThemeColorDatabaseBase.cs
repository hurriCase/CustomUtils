using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.UI.Theme.Base;

namespace CustomUtils.Runtime.UI.Theme.Databases
{
    internal abstract class ThemeColorDatabaseBase<TDatabase, TTheme, TColor> : SingletonScriptableObject<TDatabase>
        where TDatabase : ThemeColorDatabaseBase<TDatabase, TTheme, TColor>
        where TTheme : class, IThemeColor<TColor>
    {
        public abstract List<TTheme> Colors { get; protected set; }

        internal List<string> GetColorNames()
        {
            if (Colors == null || Colors.Count == 0)
                return null;

            return Colors.Select(color => color.Name).ToList();
        }

        internal bool TryGetColorByName(ref string colorName, out TColor color)
        {
            color = default;
            if (Colors == null || Colors.Count == 0)
                return false;

            var currentTheme = ThemeHandler.CurrentThemeType.Value;
            foreach (var colorItem in Colors)
            {
                if (colorItem.Name != colorName)
                    continue;

                color = colorItem.Colors[currentTheme];
                return true;
            }

            var firstColor = Colors.First();
            color = firstColor.Colors[currentTheme];
            colorName = firstColor.Name;
            return true;
        }
    }
}