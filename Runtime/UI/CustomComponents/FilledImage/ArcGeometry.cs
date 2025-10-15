using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct ArcGeometry
    {
        internal Vector2[] InnerPoints { get; }
        internal Vector2[] OuterPoints { get; }
        internal int SegmentCount { get; }

        internal ArcGeometry(float endRadians, float startRadians, Vector2 center, float innerRadius, float outerRadius,
            int segmentsPerRadian)
        {
            var arcLengthInRadians = Mathf.Abs(endRadians - startRadians);
            var segmentCount = Mathf.FloorToInt(segmentsPerRadian * arcLengthInRadians);

            var innerPoints = new Vector2[segmentCount + 1];
            var outerPoints = new Vector2[segmentCount + 1];

            for (var i = 0; i <= segmentCount; i++)
            {
                var angle = Mathf.Lerp(startRadians, endRadians, (float)i / segmentCount);
                var direction = angle.GetDirectionFromAngle();

                innerPoints[i] = center + direction * innerRadius;
                outerPoints[i] = center + direction * outerRadius;
            }

            InnerPoints = innerPoints;
            OuterPoints = outerPoints;
            SegmentCount = segmentCount;
        }
    }
}