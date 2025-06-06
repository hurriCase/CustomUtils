#if IS_RECTTRANSFORM_EXTENDED_ENABLED
namespace CustomUtils.Editor.UI.CustomRectTransform
{
    internal struct Margins
    {
        internal float Left { get; set; }
        internal float Right { get; set; }
        internal float Top { get; set; }
        internal float Bottom { get; set; }

        internal static Margins Zero => new();
    }
}
#endif