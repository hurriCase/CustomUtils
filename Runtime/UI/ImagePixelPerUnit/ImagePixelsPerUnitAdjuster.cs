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
        [field: SerializeField, PixelPerUnitPopup] internal PixelPerUnitData BackgroundType { get; set; }
        [field: SerializeField] internal DimensionType DimensionType { get; set; }

        private RectTransform _rectTransform;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();

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
            if (BackgroundType.CornerSize == 0)
                return;

            var (spriteCornerSize, rectSize) = DimensionType switch
            {
                DimensionType.Width => (Image.sprite.border.x, _rectTransform.rect.size.x),
                DimensionType.Height => (Image.sprite.border.y, _rectTransform.rect.size.y),
                _ => (1f, 1f)
            };

            var desiredCornerSize = rectSize / BackgroundType.CornerSize;
            Image.pixelsPerUnitMultiplier = spriteCornerSize / desiredCornerSize;
        }
    }
}