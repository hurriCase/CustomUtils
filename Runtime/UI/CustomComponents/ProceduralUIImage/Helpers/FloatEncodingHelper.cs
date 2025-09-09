using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Helpers
{
    internal static class FloatEncodingHelper
    {
        private static readonly Vector2 _decodeDot = new(1.0f, 1f / 65535.0f);

        internal static float EncodeFloats_0_1_16_16(float a, float b)
        {
            var encodedValues = new Vector2(
                Mathf.Floor(a * 65534) / 65535f,
                Mathf.Floor(b * 65534) / 65535f
            );

            return Vector2.Dot(encodedValues, _decodeDot);
        }
    }
}