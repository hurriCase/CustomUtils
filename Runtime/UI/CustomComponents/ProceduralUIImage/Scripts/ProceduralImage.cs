using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Helpers;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Modifiers;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts
{
    [UsedImplicitly]
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Procedural Image")]
    public class ProceduralImage : Image
    {
        [UsedImplicitly]
        [field: SerializeField] public SerializableReactiveProperty<float> BorderRatio { get; set; } = new();

        [UsedImplicitly]
        [field: SerializeField] public SerializableReactiveProperty<float> FalloffDistance { get; set; } = new();

        private ProceduralImageModifier _modifier;

        private ProceduralImageModifier Modifier
        {
            get
            {
                if (!_modifier)
                    _modifier = TryGetComponent<ProceduralImageModifier>(out var existingModifier)
                        ? existingModifier
                        : AddNewModifier(typeof(AdaptiveBorderModifier));

                return _modifier;
            }
        }

        [UsedImplicitly]
        public bool SetModifierType(System.Type modifierType)
        {
            if (TryGetComponent<ProceduralImageModifier>(out var currentModifier)
                && currentModifier.GetType() == modifierType)
                return true;

            DestroyImmediate(currentModifier);
            _modifier = null;

            AddNewModifier(modifierType);

            SetAllDirty();

            return true;
        }

        private ProceduralImageModifier AddNewModifier(System.Type modifierType)
        {
            gameObject.AddComponent(modifierType);
            _modifier = GetComponent<ProceduralImageModifier>();
            return _modifier;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BorderRatio.SubscribeAndRegister(this, static self => self.SetVerticesDirty());
            FalloffDistance.SubscribeAndRegister(this, static self => self.SetVerticesDirty());

            Init();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_OnDirtyVertsCallback -= OnVerticesDirty;
        }

        private void Init()
        {
            FixTexCoordsInCanvas();

            m_OnDirtyVertsCallback += OnVerticesDirty;
            preserveAspect = false;
            material = null;

            if (!sprite)
                sprite = EmptySpriteHelper.GetSprite();
        }

        private void OnVerticesDirty()
        {
            if (!sprite)
                sprite = EmptySpriteHelper.GetSprite();
        }

        private void FixTexCoordsInCanvas()
        {
            if (canvas)
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1 |
                                                   AdditionalCanvasShaderChannels.TexCoord2 |
                                                   AdditionalCanvasShaderChannels.TexCoord3;
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Application.isPlaying is false)
                UpdateGeometry();
        }
#endif

        private Vector4 FixRadius(Vector4 cornerRadius)
        {
            cornerRadius = cornerRadius.ClampToPositive();
            var scaleFactor = rectTransform.rect.CalculateScaleFactorForBounds(cornerRadius);
            return cornerRadius * scaleFactor;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            EncodeAllInfoIntoVertices(toFill, CalculateInfo());
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            FixTexCoordsInCanvas();
        }

        private ProceduralImageInfo CalculateInfo()
        {
            var imageRect = GetPixelAdjustedRect();
            var pixelSize = 1f / Mathf.Max(0, FalloffDistance.Value);

            var radius = FixRadius(Modifier.CalculateRadius(imageRect));

            var minSide = Mathf.Min(imageRect.width, imageRect.height);

            var info = new ProceduralImageInfo(imageRect.width + FalloffDistance.Value,
                imageRect.height + FalloffDistance.Value,
                FalloffDistance.Value, pixelSize, radius / minSide, BorderRatio.Value);

            return info;
        }

        private void EncodeAllInfoIntoVertices(VertexHelper vertexHelper, ProceduralImageInfo info)
        {
            var vert = new UIVertex();

            var uv1 = new Vector2(info.Width, info.Height);
            var uv2 = new Vector2(
                FloatEncodingHelper.EncodeFloats_0_1_16_16(info.Radius.x, info.Radius.y),
                FloatEncodingHelper.EncodeFloats_0_1_16_16(info.Radius.z, info.Radius.w)
            );

            var uv3 = new Vector2(info.BorderWidth == 0 ? 1 : Mathf.Clamp01(info.BorderWidth), info.PixelSize);

            for (var i = 0; i < vertexHelper.currentVertCount; i++)
            {
                vertexHelper.PopulateUIVertex(ref vert, i);

                vert.position += ((Vector3)vert.uv0 - new Vector3(0.5f, 0.5f)) * info.FallOffDistance;

                vert.uv1 = uv1;
                vert.uv2 = uv2;
                vert.uv3 = uv3;

                vertexHelper.SetUIVertex(vert, i);
            }
        }

        public override Material material
        {
            get => !m_Material ? MaterialHelper.GetMaterial() : base.material;
            set => base.material = value;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            OnEnable();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            FalloffDistance.Value = Mathf.Max(0, FalloffDistance.Value);

            BorderRatio.Value = Mathf.Max(0, BorderRatio.Value);
        }
#endif
    }
}