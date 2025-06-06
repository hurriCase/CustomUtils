#if IS_RECTTRANSFORM_EXTENDED_ENABLED
namespace CustomUtils.Editor.UI.CustomRectTransform
{
    internal struct LayoutData
    {
        internal float ParentWidth { get; set; }
        internal float ParentHeight { get; set; }
        internal float ContentWidth { get; set; }
        internal float ContentHeight { get; set; }
        internal float LeftMargin { get; set; }
        internal float RightMargin { get; set; }
        internal float TopMargin { get; set; }
        internal float BottomMargin { get; set; }

        internal static LayoutData Empty => new();
    }
}
#endif