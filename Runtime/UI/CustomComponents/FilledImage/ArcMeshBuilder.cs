using System.Collections.Generic;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    internal sealed class ArcMeshBuilder
    {
        private const float HalfCircleRadians = Mathf.PI * 0.5f;
        private readonly List<Vector2> _capPointsCache = new();

        internal void BuildMesh(VertexHelper vertexHelper, ArcGeometry geometry, Color color, int capResolution)
        {
            vertexHelper.Clear();

            BuildArcSegments(vertexHelper, geometry, color);

            if (geometry.CapGeometry.Radius <= 0f)
                return;

            var cap = geometry.CapGeometry;
            BuildSingleCap(vertexHelper, cap.StartCenter, cap.Radius, cap.StartAngle, color, capResolution);
            BuildSingleCap(vertexHelper, cap.EndCenter, cap.Radius, cap.EndAngle, color, capResolution);
        }

        private void BuildArcSegments(VertexHelper vertexHelper, ArcGeometry geometry, Color color)
        {
            for (var i = 0; i < geometry.SegmentCount; i++)
            {
                var vertexStart = vertexHelper.currentVertCount;

                vertexHelper.AddVert(geometry.InnerPoints[i], color, Vector2.zero);
                vertexHelper.AddVert(geometry.OuterPoints[i], color, Vector2.zero);
                vertexHelper.AddVert(geometry.InnerPoints[i + 1], color, Vector2.zero);
                vertexHelper.AddVert(geometry.OuterPoints[i + 1], color, Vector2.zero);

                vertexHelper.AddTriangle(vertexStart, vertexStart + 1, vertexStart + 2);
                vertexHelper.AddTriangle(vertexStart + 1, vertexStart + 3, vertexStart + 2);
            }
        }

        private void BuildSingleCap(
            VertexHelper vertexHelper,
            Vector2 center,
            float radius,
            float baseAngle,
            Color color,
            int resolution)
        {
            _capPointsCache.Clear();
            _capPointsCache.Add(center);

            for (var i = 0; i <= resolution; i++)
            {
                var angle = baseAngle + Mathf.Lerp(-HalfCircleRadians, HalfCircleRadians, (float)i / resolution);
                _capPointsCache.Add(center + angle.GetDirectionFromAngle() * radius);
            }

            var vertexStart = vertexHelper.currentVertCount;

            foreach (var point in _capPointsCache)
                vertexHelper.AddVert(point, color, Vector2.zero);

            for (var i = 1; i < _capPointsCache.Count - 1; i++)
                vertexHelper.AddTriangle(vertexStart, vertexStart + i, vertexStart + i + 1);
        }
    }
}