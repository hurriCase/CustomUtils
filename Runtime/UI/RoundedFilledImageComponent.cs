using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    internal class RoundedFilledImageComponent : Image
    {
        [field: SerializeField] internal bool RoundedCaps { get; set; } = true;
        [field: SerializeField, Range(3, 36)] internal int RoundedCapResolution { get; set; } = 8;
        [field: SerializeField, Range(0, 359)] internal float CustomFillOrigin { get; set; }
        [field: SerializeField] internal bool UseCustomFillOrigin { get; set; }
        [field: SerializeField, Range(0.01f, 0.5f)] internal float ThicknessRatio { get; set; } = 0.2f;

        protected override void Reset()
        {
            base.Reset();

            sprite = ResourceReferences.Instance.SquareSprite;
            type = Type.Filled;
            fillMethod = FillMethod.Radial360;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetAllDirty();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            SetAllDirty();
        }

        public override void SetAllDirty()
        {
            base.SetAllDirty();

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            if (RoundedCaps is false || type != Type.Filled || fillMethod != FillMethod.Radial360)
            {
                base.OnPopulateMesh(vertexHelper);
                return;
            }

            vertexHelper.Clear();

            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;

            var pivot = rectTransform.pivot;
            var center = new Vector2(
                -width * pivot.x + width * 0.5f,
                -height * pivot.y + height * 0.5f
            );

            var radius = Mathf.Min(width, height) * 0.5f;
            var thickness = radius * ThicknessRatio;
            var innerRadius = radius - thickness;

            var fullAngle = fillClockwise ? 360f : -360f;

            float startAngle;
            if (UseCustomFillOrigin)
                startAngle = CustomFillOrigin;
            else
                startAngle = fillOrigin switch
                {
                    0 => 270f,
                    1 => 0f,
                    2 => 90f,
                    3 => 180f,
                    _ => 0f
                };

            var endAngle = startAngle + fullAngle * fillAmount;

            var startRad = startAngle * Mathf.Deg2Rad;
            var endRad = endAngle * Mathf.Deg2Rad;

            GenerateArc(vertexHelper, center, innerRadius, radius, startRad, endRad, color);

            if ((fillAmount > 0.001f) is false || (fillAmount < 0.999f) is false)
                return;

            var endDirection = new Vector2(Mathf.Cos(endRad), Mathf.Sin(endRad));
            var endCenter = center + endDirection * (innerRadius + thickness * 0.5f);

            GenerateRoundedCap(vertexHelper, endCenter, thickness * 0.5f,
                endRad + (fillClockwise ? Mathf.PI * 0.5f : -Mathf.PI * 0.5f), color);

            var startDirection = new Vector2(Mathf.Cos(startRad), Mathf.Sin(startRad));
            var startCenter = center + startDirection * (innerRadius + thickness * 0.5f);

            GenerateRoundedCap(vertexHelper, startCenter, thickness * 0.5f,
                startRad - (fillClockwise ? Mathf.PI * 0.5f : -Mathf.PI * 0.5f), color);
        }

        private void GenerateArc(VertexHelper vertexHelper, Vector2 center, float innerRadius, float outerRadius,
            float startRad, float endRad, Color color)
        {
            var innerPoints = new List<Vector2>();
            var outerPoints = new List<Vector2>();

            var arcLength = Mathf.Abs(endRad - startRad);
            var segments = Mathf.Max(6, Mathf.FloorToInt(arcLength * 20));

            for (var i = 0; i <= segments; i++)
            {
                var angle = Mathf.Lerp(startRad, endRad, (float)i / segments);
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                innerPoints.Add(center + direction * innerRadius);
                outerPoints.Add(center + direction * outerRadius);
            }

            var vertexCount = vertexHelper.currentVertCount;

            for (var i = 0; i < segments; i++)
            {
                vertexHelper.AddVert(innerPoints[i], color, Vector2.zero);
                vertexHelper.AddVert(outerPoints[i], color, Vector2.zero);
                vertexHelper.AddVert(innerPoints[i + 1], color, Vector2.zero);
                vertexHelper.AddVert(outerPoints[i + 1], color, Vector2.zero);

                vertexHelper.AddTriangle(vertexCount, vertexCount + 1, vertexCount + 2);
                vertexHelper.AddTriangle(vertexCount + 1, vertexCount + 3, vertexCount + 2);

                vertexCount += 4;
            }
        }

        private void GenerateRoundedCap(VertexHelper vertexHelper, Vector2 center, float radius, float angle,
            Color color)
        {
            var segments = RoundedCapResolution;
            var points = new List<Vector2>
            {
                center
            };

            for (var i = 0; i <= segments; i++)
            {
                var segmentAngle = angle + Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, (float)i / segments);
                var direction = new Vector2(Mathf.Cos(segmentAngle), Mathf.Sin(segmentAngle));
                points.Add(center + direction * radius);
            }

            var vertexStart = vertexHelper.currentVertCount;

            foreach (var point in points)
                vertexHelper.AddVert(point, color, Vector2.zero);

            for (var i = 1; i < points.Count - 1; i++)
                vertexHelper.AddTriangle(vertexStart, vertexStart + i, vertexStart + i + 1);
        }
    }
}