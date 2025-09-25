using CustomUtils.Runtime.Extensions.Observables;
using CustomUtils.Runtime.UI.Theme.Databases.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    internal abstract class GenericColorModifierBase<TColor> : ColorModifierBase
    {
        protected abstract IThemeDatabase<TColor> ThemeDatabase { get; }

        [SerializeField, HideInInspector] protected string currentColorName;

        protected virtual void Awake()
        {
            ThemeHandler.CurrentThemeType.SubscribeUntilDestroy(this, self => self.UpdateColor(self.currentColorName));
        }

        internal override void UpdateColor(string colorName)
        {
            if (ThemeDatabase.TryGetColorByName(colorName, out var color) is false)
                return;

            OnUpdateColor(color);
            currentColorName = colorName;
        }

        protected abstract void OnUpdateColor(TColor color);
    }
}