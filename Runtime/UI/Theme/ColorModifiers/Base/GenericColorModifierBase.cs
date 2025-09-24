using System;
using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.Databases.Base;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    [Serializable]
    internal abstract class GenericColorModifierBase<TColor> : ColorModifierBase
    {
        internal abstract IThemeDatabase<TColor> ThemeDatabase { get; }

        protected string currentColorName;

        protected virtual void OnEnable()
        {
            ThemeHandler.CurrentThemeType.SubscribeUntilDisable(this, self => self.UpdateColor(self.currentColorName));
        }

        internal override void UpdateColor(string colorName)
        {
            if (ThemeDatabase.TryGetColorByName(colorName, out var color) is false)
                return;

            OnApplyColor(color);
            currentColorName = colorName;
        }

        protected abstract void OnApplyColor(TColor color);
    }
}