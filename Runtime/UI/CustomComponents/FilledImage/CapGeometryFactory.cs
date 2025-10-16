using CustomUtils.Runtime.UI.CustomComponents.FilledImage.Modifier;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal static class CapGeometryFactory
    {
        internal static CapGeometryBase CreateModifier(CapGeometryType geometryType, GameObject gameObject)
            => geometryType switch
            {
                CapGeometryType.Rounded => gameObject.AddComponent<RoundedCapGeometry>(),
                CapGeometryType.Custom => gameObject.AddComponent<CustomCapGeometry>(),
                _ => null
            };
    }
}