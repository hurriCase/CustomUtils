using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct ArcGeometry
    {
        internal Vector2[] InnerPoints { get; }
        internal Vector2[] OuterPoints { get; }
        internal int SegmentCount { get; }
        internal CapGeometry CapGeometry { get; }

        internal ArcGeometry(Vector2[] innerPoints, Vector2[] outerPoints, int segmentCount, CapGeometry capGeometry)
        {
            InnerPoints = innerPoints;
            OuterPoints = outerPoints;
            SegmentCount = segmentCount;
            CapGeometry = capGeometry;
        }
    }
}