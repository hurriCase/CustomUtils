using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct ArcGeometry
    {
        internal Vector2[] InnerPoints { get; }
        internal Vector2[] OuterPoints { get; }
        internal bool HasRoundedCaps { get; }
        internal Vector2[] StartCapPoints { get; }
        internal Vector2[] EndCapPoints { get; }
        internal int SegmentCount { get; }

        internal ArcGeometry(
            ArcParameters parameters,
            int arcResolution,
            bool hasRoundedCaps,
            Vector2[] startCapPoints,
            Vector2[] endCapPoints)
        {
            var arcLengthInRadians = Mathf.Abs(parameters.EndRadians - parameters.StartRadians);
            var segmentCount = Mathf.FloorToInt(arcResolution * arcLengthInRadians);

            var innerPoints = new Vector2[segmentCount + 1];
            var outerPoints = new Vector2[segmentCount + 1];

            for (var i = 0; i <= segmentCount; i++)
            {
                var angle = Mathf.Lerp(parameters.StartRadians, parameters.EndRadians, (float)i / segmentCount);
                var direction = angle.GetDirectionFromAngle();

                innerPoints[i] = parameters.Center + direction * parameters.InnerRadius;
                outerPoints[i] = parameters.Center + direction * parameters.OuterRadius;
            }

            InnerPoints = innerPoints;
            OuterPoints = outerPoints;
            HasRoundedCaps = hasRoundedCaps;
            StartCapPoints = startCapPoints;
            EndCapPoints = endCapPoints;
            SegmentCount = segmentCount;
        }
    }
}