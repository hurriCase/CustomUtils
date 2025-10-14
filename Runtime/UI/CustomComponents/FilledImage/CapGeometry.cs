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

        internal CapGeometry(Vector2 startCenter, Vector2 endCenter, float radius, float startAngle, float endAngle)
        {
            StartCenter = startCenter;
            EndCenter = endCenter;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }
    }
}