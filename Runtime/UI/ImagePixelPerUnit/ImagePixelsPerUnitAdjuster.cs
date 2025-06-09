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

        private Image _image;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();

            _image.type = Image.Type.Sliced;

            UpdateImagePixelPerUnit();
        }

        private void Update()
        {
            UpdateImagePixelPerUnit();
        }

        private void UpdateImagePixelPerUnit()
        {
            if (!_image.sprite)
                return;

            var spriteCornerSize = _image.sprite.border.x;
            var cornerToSizeRatio = _rectTransform.rect.size.x / BackgroundType.CornerRatio;
            _image.pixelsPerUnitMultiplier = spriteCornerSize / cornerToSizeRatio;
        }
    }
}