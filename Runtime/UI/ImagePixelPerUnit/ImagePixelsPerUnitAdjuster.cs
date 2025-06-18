using CustomUtils.Runtime.CustomBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    internal sealed class ImagePixelsPerUnitAdjuster : ImageBehaviour
    {
        [field: SerializeField, PixelPerUnitPopup] internal PixelPerUnitData PixelPerUnitData { get; set; }

        private RectTransform RectTransform =>
            _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            Image.type = Image.Type.Sliced;

            UpdateImagePixelPerUnit();
        }

        private void OnValidate()
        {
            UpdateImagePixelPerUnit();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateImagePixelPerUnit();
        }

        private void UpdateImagePixelPerUnit()
        {
            if (PixelPerUnitData.IsCorrectData is false || !Image.sprite)
                return;

            var (spriteCornerSize, rectSize) = PixelPerUnitData.DimensionType switch
            {
                DimensionType.Width => (Image.sprite.border.x, RectTransform.rect.size.x),
                DimensionType.Height => (Image.sprite.border.y, RectTransform.rect.size.y),
                _ => (1f, 1f)
            };

            var desiredCornerSize = rectSize / PixelPerUnitData.CornerRatio;
            Image.pixelsPerUnitMultiplier = spriteCornerSize / desiredCornerSize;
        }
    }
}