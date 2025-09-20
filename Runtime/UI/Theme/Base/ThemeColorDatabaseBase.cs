using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.CustomTypes.Singletons;

namespace CustomUtils.Runtime.UI.Theme.Base
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

        internal bool TryGetColorByName(string name, out TColor color)
        {
            color = default;
            if (Colors == null || Colors.Count == 0)
                return false;

            foreach (var colorItem in Colors)
            {
                if (colorItem.Name != name)
                    continue;

                color = colorItem.Colors[ThemeHandler.CurrentThemeType.Value];
                return true;
            }

            return false;
        }
    }
}