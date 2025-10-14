using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.FilledImage
{
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteAlways]
    public sealed class RoundedFilledImageComponent : Image
    {
        [field: SerializeField] public bool RoundedCaps { get; set; } = true;
        [field: SerializeField, Range(0, 359)] public float CustomFillOrigin { get; set; }
        [field: SerializeField, Range(0.01f, 0.5f)] public float ThicknessRatio { get; set; }
        [field: SerializeField, Range(3, 36)] public int RoundedCapResolution { get; set; }

        private const float AlmostZeroFill = 0.001f;
        private const float AlmostFullFill = 0.999f;

        private readonly ArcGeometryCalculator _geometryCalculator = new();
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

            var hasRoundedCaps = fillAmount is > AlmostZeroFill and < AlmostFullFill && RoundedCaps;

            var geometry = _geometryCalculator.Calculate(
                rectTransform,
                ThicknessRatio,
                CustomFillOrigin,
                fillAmount,
                fillClockwise,
                hasRoundedCaps
            );

            _meshBuilder.BuildMesh(vertexHelper, geometry, color, RoundedCapResolution);
        }
    }
}