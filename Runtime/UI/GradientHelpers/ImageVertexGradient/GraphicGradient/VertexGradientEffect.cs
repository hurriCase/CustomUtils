using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.GradientHelpers.ImageVertexGradient.GraphicGradient
{
    [ExecuteAlways]
    public sealed class VertexGradientEffect : BaseMeshEffect
    {
        private const int VerticesPerQuad = 4;
        private const int BottomLeftIndex = 0;
        private const int TopLeftIndex = 1;
        private const int TopRightIndex = 2;
        private const int BottomRightIndex = 3;

        private readonly UIVertex[] _quadVertices = new UIVertex[VerticesPerQuad];

        private Color _startColor;
        private Color _endColor;
        private GradientDirection _direction;

        internal void SetGradient(Color startColor, Color endColor, GradientDirection direction)
        {
            _startColor = startColor;
            _endColor = endColor;
            _direction = direction;
        }

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (_direction == GradientDirection.None)
                return;

            var vertexCount = vertexHelper.currentVertCount;

            for (var i = 0; i < vertexCount; i += VerticesPerQuad)
            {
                if (i + BottomRightIndex >= vertexCount)
                    break;

                for (var vertexIndex = 0; vertexIndex < VerticesPerQuad; vertexIndex++)
                    vertexHelper.PopulateUIVertex(ref _quadVertices[vertexIndex], i + vertexIndex);

                ApplyColor();

                for (var vertexIndex = 0; vertexIndex < VerticesPerQuad; vertexIndex++)
                    vertexHelper.SetUIVertex(_quadVertices[vertexIndex], i + vertexIndex);
            }
        }

        private void ApplyColor()
        {
            var isHorizontal = _direction is GradientDirection.LeftToRight or GradientDirection.RightToLeft;
            var isReversed = _direction is GradientDirection.RightToLeft or GradientDirection.TopToBottom;

            var (firstColor, secondColor) = isReversed ? (_endColor, _startColor) : (_startColor, _endColor);

            _quadVertices[BottomLeftIndex].color = firstColor;
            _quadVertices[TopLeftIndex].color = isHorizontal ? firstColor : secondColor;
            _quadVertices[TopRightIndex].color = secondColor;
            _quadVertices[BottomRightIndex].color = isHorizontal ? secondColor : firstColor;
        }
    }
}