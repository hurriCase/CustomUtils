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
        [field: SerializeField, PixelPerUnitPopup] internal float CornerSize { get; set; }

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
            if (CornerSize == 0)
                return;

            var rect = RectTransform.rect;
            var rectSize = Mathf.Max(rect.size.x, rect.size.y);
            var spriteCornerSize = Mathf.Max(Image.sprite.border.x, Image.sprite.border.y);

            var desiredCornerSize = rectSize / CornerSize;
            Image.pixelsPerUnitMultiplier = spriteCornerSize / desiredCornerSize;
        }
    }
}