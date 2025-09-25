namespace CustomUtils.Runtime.UI.Theme.Databases.Base
{
    internal interface IThemeDatabase<TColor>
    {
        bool TryGetColorByName(string colorName, out TColor color);
    }
}