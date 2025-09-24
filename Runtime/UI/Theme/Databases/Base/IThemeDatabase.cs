using System.Collections.Generic;

namespace CustomUtils.Runtime.UI.Theme.Databases.Base
{
    internal interface IThemeDatabase<TColor>
    {
        List<string> GetColorNames();
        bool TryGetColorByName(string colorName, out TColor color);
        string GetFirstColorName();
    }
}