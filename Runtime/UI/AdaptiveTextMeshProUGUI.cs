using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    public sealed class AdaptiveTextMeshProUGUI : TextMeshProUGUI
    {
        [field: SerializeField] public DimensionType DimensionType { get; private set; }
        [field: SerializeField] public float BaseFontSize { get; private set; }
        [field: SerializeField] public float ReferenceSize { get; private set; }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetCategoriesText();
        }

        private void SetCategoriesText()
        {
            if (ReferenceSize == 0 || DimensionType == DimensionType.None)
                return;

            var scaleFactor = DimensionType switch
            {
                DimensionType.Width => rectTransform.rect.width / ReferenceSize,
                DimensionType.Height => rectTransform.rect.height / ReferenceSize,
                _ => 0,
            };

            if (scaleFactor <= 0)
                return;

            var adaptiveFontSize = BaseFontSize * scaleFactor;

            fontSize = adaptiveFontSize;
        }
    }
}