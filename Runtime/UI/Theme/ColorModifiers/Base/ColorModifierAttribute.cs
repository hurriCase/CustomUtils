using System;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class ColorModifierAttribute : Attribute
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global | Used by Roslyn
        internal ColorType ColorType { get; }

        internal ColorModifierAttribute(ColorType colorType)
        {
            ColorType = colorType;
        }
    }
}