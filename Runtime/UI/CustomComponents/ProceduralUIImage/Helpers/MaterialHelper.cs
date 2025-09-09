using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Helpers
{
    internal static class MaterialHelper
    {
        private static Material _materialInstance;

        internal static Material GetMaterial()
        {
            if (!_materialInstance)
                _materialInstance = new Material(Shader.Find("UI/Procedural UI Image"));

            return _materialInstance;
        }
    }
}