using CustomUtils.Runtime.Extensions;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct CapGeometry
    {
        internal Vector2 StartCenter { get; }
        internal Vector2 EndCenter { get; }
        internal float Radius { get; }
        internal float StartAngle { get; }
        internal float EndAngle { get; }

        private const float HalfCircleRadians = Mathf.PI * 0.5f;

        internal CapGeometry(Vector2 center, float innerRadius, float outerRadius, float startRadians, float endRadians)
        {
            var middleRadius = innerRadius + (outerRadius - innerRadius) * 0.5f;
            var capRadius = (outerRadius - innerRadius) * 0.5f;

            var startDirection = startRadians.GetDirectionFromAngle();
            var endDirection = endRadians.GetDirectionFromAngle();

            StartCenter = center + startDirection * middleRadius;
            EndCenter = center + endDirection * middleRadius;
            Radius = capRadius;
            StartAngle = startRadians - HalfCircleRadians;
            EndAngle = endRadians + HalfCircleRadians;
        }
    }
}