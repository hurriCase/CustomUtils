using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage
{
    internal struct ProceduralImageInfo
    {
        internal float Width { get; }
        internal float Height { get; }
        internal float FallOffDistance { get; }
        internal Vector4 NormilizedRadius { get; }
        internal float BorderWidth { get; }
        internal float PixelSize { get; }

        internal ProceduralImageInfo(
            float width,
            float height,
            float fallOffDistance,
            float pixelSize,
            Vector4 normilizedRadius,
            float borderWidth)
        {
            Width = Mathf.Abs(width);
            Height = Mathf.Abs(height);
            FallOffDistance = Mathf.Max(0, fallOffDistance);
            NormilizedRadius = normilizedRadius;
            BorderWidth = Mathf.Max(borderWidth, 0);
            PixelSize = Mathf.Max(0, pixelSize);
        }
    }
}