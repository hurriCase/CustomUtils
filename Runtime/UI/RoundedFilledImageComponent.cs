using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteInEditMode]
    public sealed class RoundedFilledImageComponent : Image
    {
        [field: SerializeField] public bool RoundedCaps { get; set; } = true;
        [field: SerializeField, Range(3, 36)] public int RoundedCapResolution { get; set; } = 8;
        [field: SerializeField, Range(0, 359)] public float CustomFillOrigin { get; set; }
        [field: SerializeField, Range(0.01f, 0.5f)] public float ThicknessRatio { get; set; } = 0.2f;

        private readonly List<Vector2> _innerPointsCashed = new();
        private readonly List<Vector2> _outerPointsCashed = new();
        private readonly List<Vector2> _pointsCashed = new();

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
            if (type != Type.Filled || fillMethod != FillMethod.Radial360)
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
            var startAngle = CustomFillOrigin;
            var endAngle = startAngle + fullAngle * fillAmount;

            var startRad = startAngle * Mathf.Deg2Rad;
            var endRad = endAngle * Mathf.Deg2Rad;

            GenerateArc(vertexHelper, center, innerRadius, radius, startRad, endRad, color);

            if ((fillAmount > 0.001f) is false || (fillAmount < 0.999f) is false || RoundedCaps is false)
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
            var arcLength = Mathf.Abs(endRad - startRad);
            var segments = Mathf.Max(6, Mathf.FloorToInt(arcLength * 20));

            var actualInnerRadius = innerRadius;
            var actualOuterRadius = outerRadius;

            if (RoundedCaps is false)
            {
                var cornerRadius = (outerRadius - innerRadius) * 0.2f;
                actualInnerRadius = innerRadius + cornerRadius;
                actualOuterRadius = outerRadius - cornerRadius;
            }

            _innerPointsCashed.Clear();
            _outerPointsCashed.Clear();

            for (var i = 0; i <= segments; i++)
            {
                var angle = Mathf.Lerp(startRad, endRad, (float)i / segments);
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                _innerPointsCashed.Add(center + direction * actualInnerRadius);
                _outerPointsCashed.Add(center + direction * actualOuterRadius);
            }

            var vertexCount = vertexHelper.currentVertCount;

            for (var i = 0; i < segments; i++)
            {
                vertexHelper.AddVert(_innerPointsCashed[i], color, Vector2.zero);
                vertexHelper.AddVert(_outerPointsCashed[i], color, Vector2.zero);
                vertexHelper.AddVert(_innerPointsCashed[i + 1], color, Vector2.zero);
                vertexHelper.AddVert(_outerPointsCashed[i + 1], color, Vector2.zero);

                vertexHelper.AddTriangle(vertexCount, vertexCount + 1, vertexCount + 2);
                vertexHelper.AddTriangle(vertexCount + 1, vertexCount + 3, vertexCount + 2);

                vertexCount += 4;
            }
        }

        private void GenerateRoundedCap(VertexHelper vertexHelper, Vector2 center, float radius, float angle,
            Color color)
        {
            var segments = RoundedCapResolution;
            _pointsCashed.Clear();
            _pointsCashed.Add(center);

            for (var i = 0; i <= segments; i++)
            {
                var segmentAngle = angle + Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, (float)i / segments);
                var direction = new Vector2(Mathf.Cos(segmentAngle), Mathf.Sin(segmentAngle));
                _pointsCashed.Add(center + direction * radius);
            }

            var vertexStart = vertexHelper.currentVertCount;

            foreach (var point in _pointsCashed)
                vertexHelper.AddVert(point, color, Vector2.zero);

            for (var i = 1; i < _pointsCashed.Count - 1; i++)
                vertexHelper.AddTriangle(vertexStart, vertexStart + i, vertexStart + i + 1);
        }
    }
}