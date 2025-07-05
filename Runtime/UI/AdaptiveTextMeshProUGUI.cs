using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    public sealed class AdaptiveTextMeshProUGUI : TextMeshProUGUI
    {
        [field: SerializeField] public float BaseFontSize { get; private set; }
        [field: SerializeField] public float ReferenceWidth { get; private set; }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetCategoriesText();
        }

        private void SetCategoriesText()
        {
            var adaptiveFontSize = BaseFontSize * rectTransform.rect.width / ReferenceWidth;

            fontSize = adaptiveFontSize;
        }
    }
}