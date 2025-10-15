using CustomUtils.Runtime.Constants;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteAlways]
    public sealed class RoundedFilledImage : Image
    {
        [field: SerializeField] public bool IsRoundedCaps { get; set; }
        [field: SerializeField, Range(0, 359)] public float CustomFillOrigin { get; set; }
        [field: SerializeField, Range(0.01f, 0.5f)] public float ThicknessRatio { get; set; } = 0.2f;
        [field: SerializeField, Range(3, 36)] public int RoundedCapResolution { get; set; } = 15;
        [field: SerializeField] public float SegmentsPerRadian { get; set; } = 30;

        private const float HalfPivot = 0.5f;
        private const float AlmostZeroFill = 0.001f;
        private const float AlmostFullFill = 0.999f;

        private readonly ArcMeshBuilder _meshBuilder = new();

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            sprite = ResourceReferences.Instance.SquareSprite;
            type = Type.Filled;
            fillMethod = FillMethod.Radial360;
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            SetAllDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            SetAllDirty();
        }
#endif

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

            var geometry = CalculateGeometry();

            _meshBuilder.BuildMesh(vertexHelper, geometry, color, RoundedCapResolution);
        }

        private ArcGeometry CalculateGeometry()
        {
            var rect = rectTransform.rect;
            var center = rect.center;
            var outerRadius = Mathf.Min(rect.width, rect.height) * HalfPivot;
            var thickness = outerRadius * ThicknessRatio;
            var innerRadius = outerRadius - thickness;

            var (startRadians, endRadians) = CalculateCapAngles();
            var arcLengthInRadians = Mathf.Abs(endRadians - startRadians);
            var segmentCount = Mathf.FloorToInt(SegmentsPerRadian * arcLengthInRadians);

            var innerPoints = new Vector2[segmentCount + 1];
            var outerPoints = new Vector2[segmentCount + 1];

            for (var i = 0; i <= segmentCount; i++)
            {
                var angle = Mathf.Lerp(startRadians, endRadians, (float)i / segmentCount);
                var direction = angle.GetDirectionFromAngle();

                innerPoints[i] = center + direction * innerRadius;
                outerPoints[i] = center + direction * outerRadius;
            }

            var hasRoundedCaps = IsRoundedCaps && fillAmount is > AlmostZeroFill and < AlmostFullFill;
            var capGeometry = hasRoundedCaps
                ? new CapGeometry(center, innerRadius, outerRadius, startRadians, endRadians)
                : default;

            return new ArcGeometry(innerPoints, outerPoints, segmentCount, capGeometry);
        }

        private (float startRadians, float endRadians) CalculateCapAngles()
        {
            var endAngle = CustomFillOrigin + MathConstants.FullCircleDegrees * fillAmount;
            return (CustomFillOrigin.ToRadians(), endAngle.ToRadians());
        }
    }
}