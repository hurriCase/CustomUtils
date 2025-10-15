using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal readonly struct ArcParameters
    {
        internal float StartRadians { get; }
        internal float EndRadians { get; }
        internal Vector2 Center { get; }
        internal float InnerRadius { get; }
        internal float OuterRadius { get; }

        internal ArcParameters(
            float startRadians,
            float endRadians,
            Vector2 center,
            float innerRadius,
            float outerRadius)
        {
            StartRadians = startRadians;
            EndRadians = endRadians;
            Center = center;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
    }
}