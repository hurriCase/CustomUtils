using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    internal sealed class ImagePixelsPerUnitAdjuster : MonoBehaviour
    {
        [field: SerializeField, PixelPerUnitPopup] internal PixelPerUnitData BackgroundType { get; set; }
        [field: SerializeField] internal DimensionType DimensionType { get; set; }

        private Image _image;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();

            _image.type = Image.Type.Sliced;

            UpdateImagePixelPerUnit();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateImagePixelPerUnit();
        }

        private void UpdateImagePixelPerUnit()
        {
            if (!_image.sprite || BackgroundType.CornerRatio == 0 || string.IsNullOrWhiteSpace(BackgroundType.Name))
                return;

            var (spriteCornerSize, rectSize) = DimensionType switch
            {
                DimensionType.Width => (_image.sprite.border.x, _rectTransform.rect.size.x),
                DimensionType.Height => (_image.sprite.border.y, _rectTransform.rect.size.y),
                _ => (1f, 1f)
            };

            var cornerToSizeRatio = rectSize / BackgroundType.CornerRatio;
            _image.pixelsPerUnitMultiplier = spriteCornerSize / cornerToSizeRatio;
        }
    }
}