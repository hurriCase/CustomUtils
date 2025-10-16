using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage.Modifier
{
    internal abstract class CapGeometryBase : MonoBehaviour
    {
        internal abstract Vector2[] CreateStartCap(CapParameters parameters, float startRadians);
        internal abstract Vector2[] CreateEndCap(CapParameters parameters, float endRadians);
    }
}