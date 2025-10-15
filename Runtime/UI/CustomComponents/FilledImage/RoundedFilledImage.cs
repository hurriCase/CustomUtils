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
        [field: SerializeField, Range(0, 359)] public float CustomFillOrigin { get; set; }
        [field: SerializeField, Range(0.01f, 0.5f)] public float ThicknessRatio { get; set; } = 0.2f;
        [field: SerializeField, Range(1, 100)] public int ArcResolutionPerRadian { get; set; } = 15;

        [field: SerializeField] public bool IsRoundedCaps { get; set; }
        [field: SerializeField, Range(3, 36)] public int RoundedCapResolution { get; set; } = 15;

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

            var arcGeometry = CalculateGeometry();
            _meshBuilder.BuildMesh(vertexHelper, arcGeometry, color);
        }

        private ArcGeometry CalculateGeometry()
        {
            var rect = rectTransform.rect;
            var center = rect.center;
            var outerRadius = Mathf.Min(rect.width, rect.height) * 0.5f;
            var thickness = outerRadius * ThicknessRatio;
            var innerRadius = outerRadius - thickness;

            var (startRadians, endRadians) = CalculateCapAngles();

            var hasRoundedCaps = IsRoundedCaps && fillAmount is > AlmostZeroFill and < AlmostFullFill;

            var capParams = new CapParameters(center, innerRadius, outerRadius, RoundedCapResolution);
            var startCap = CapGeometry.CreateStartCap(hasRoundedCaps, capParams, startRadians);
            var endCap = CapGeometry.CreateEndCap(hasRoundedCaps, capParams, endRadians);

            var arcParameters = new ArcParameters(endRadians, startRadians, center, innerRadius, outerRadius);
            return new ArcGeometry(arcParameters, ArcResolutionPerRadian, startCap, endCap);
        }

        private (float startRadians, float endRadians) CalculateCapAngles()
        {
            var endAngle = CustomFillOrigin + MathConstants.FullCircleDegrees * fillAmount;
            return (CustomFillOrigin.ToRadians(), endAngle.ToRadians());
        }
    }
}