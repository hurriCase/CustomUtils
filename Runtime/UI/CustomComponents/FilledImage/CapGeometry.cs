using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct CapGeometry
    {
        internal Vector2[] StartCapPoints { get; }
        internal Vector2[] EndCapPoints { get; }

        internal bool HasCap => StartCapPoints.Length > 0 || EndCapPoints.Length > 0;

        private const float HalfCircleRadians = Mathf.PI * 0.5f;

        internal CapGeometry(Vector2 center, float innerRadius, float outerRadius, float startRadians, float endRadians
            , int resolution)
        {
            var middleRadius = innerRadius + (outerRadius - innerRadius) * 0.5f;
            var capRadius = (outerRadius - innerRadius) * 0.5f;

            var startDirection = startRadians.GetDirectionFromAngle();
            var endDirection = endRadians.GetDirectionFromAngle();

            StartCapPoints = new Vector2[resolution + 1];
            var startCenter = center + startDirection * middleRadius;
            var startAngle = startRadians - HalfCircleRadians;
            for (var i = 0; i <= resolution; i++)
            {
                var angle = startAngle + Mathf.Lerp(-HalfCircleRadians, HalfCircleRadians, (float)i / resolution);
                StartCapPoints[i] = startCenter + angle.GetDirectionFromAngle() * capRadius;
            }

            EndCapPoints = new Vector2[resolution + 1];
            var endCenter = center + endDirection * middleRadius;
            var endAngle = endRadians + HalfCircleRadians;
            for (var i = 0; i <= resolution; i++)
            {
                var angle = endAngle + Mathf.Lerp(-HalfCircleRadians, HalfCircleRadians, (float)i / resolution);
                EndCapPoints[i] = endCenter + angle.GetDirectionFromAngle() * capRadius;
            }
        }
    }
}