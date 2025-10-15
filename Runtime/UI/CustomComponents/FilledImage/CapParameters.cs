using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct CapParameters
    {
        internal Vector2 Center { get; }
        internal float InnerRadius { get; }
        internal float OuterRadius { get; }
        internal int Resolution { get; }

        internal CapParameters(Vector2 center, float innerRadius, float outerRadius, int resolution)
        {
            Center = center;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            Resolution = resolution;
        }
    }
}