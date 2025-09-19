using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.VertexGradient
{
    [ExecuteAlways]
    internal sealed class VertexGradientEffect : BaseMeshEffect
    {
        private Color _startColor;
        private Color _endColor;
        private GradientDirection _direction;

        internal void SetGradient(Color startColor, Color endColor, GradientDirection direction)
        {
            _startColor = startColor;
            _endColor = endColor;
            _direction = direction;
        }

        internal Gradient GetCurrentGradient()
        {
            var gradient = new Gradient();

            var colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(_startColor, 0f);
            colorKeys[1] = new GradientColorKey(_endColor, 1f);

            var alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(_startColor.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(_endColor.a, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (_direction == GradientDirection.None)
                return;

            var vertexCount = vh.currentVertCount;

            for (var i = 0; i < vertexCount; i += 4)
            {
                if (i + 3 >= vertexCount) break;

                var bottomLeft = new UIVertex();
                var topLeft = new UIVertex();
                var topRight = new UIVertex();
                var bottomRight = new UIVertex();

                vh.PopulateUIVertex(ref bottomLeft, i);
                vh.PopulateUIVertex(ref topLeft, i + 1);
                vh.PopulateUIVertex(ref topRight, i + 2);
                vh.PopulateUIVertex(ref bottomRight, i + 3);

                ApplyColor(ref topLeft, ref topRight, ref bottomRight, ref bottomLeft);

                vh.SetUIVertex(bottomLeft, i);
                vh.SetUIVertex(topLeft, i + 1);
                vh.SetUIVertex(topRight, i + 2);
                vh.SetUIVertex(bottomRight, i + 3);
            }
        }

        private void ApplyColor(
            ref UIVertex topLeft,
            ref UIVertex topRight,
            ref UIVertex bottomRight,
            ref UIVertex bottomLeft)
        {
            var isHorizontal = _direction is GradientDirection.LeftToRight or GradientDirection.RightToLeft;
            var isReversed = _direction is GradientDirection.RightToLeft or GradientDirection.TopToBottom;

            var (firstColor, secondColor) = isReversed ? (_endColor, _startColor) : (_startColor, _endColor);

            if (isHorizontal)
            {
                topLeft.color = bottomLeft.color = firstColor;
                topRight.color = bottomRight.color = secondColor;
                return;
            }

            bottomLeft.color = bottomRight.color = firstColor;
            topLeft.color = topRight.color = secondColor;
        }
    }
}