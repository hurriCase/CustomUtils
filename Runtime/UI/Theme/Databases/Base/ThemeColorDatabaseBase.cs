using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Databases.Base
{
    internal abstract class ThemeColorDatabaseBase<TDatabase, TTheme, TColor> :
        SingletonScriptableObject<TDatabase>, IThemeDatabase<TColor>
        where TDatabase : ThemeColorDatabaseBase<TDatabase, TTheme, TColor>
        where TTheme : class, IThemeColor<TColor>
    {
        [field: SerializeField] public List<TTheme> Colors { get; protected set; }

        public List<string> GetColorNames()
        {
            if (Colors == null || Colors.Count == 0)
                return null;

            return Colors.Select(color => color.Name).ToList();
        }

        public bool TryGetColorByName(string colorName, out TColor color)
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

            return false;
        }
    }
}