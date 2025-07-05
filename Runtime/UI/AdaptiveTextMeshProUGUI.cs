using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    public sealed class AdaptiveTextMeshProUGUI : TextMeshProUGUI
    {
        [field: SerializeField] public DimensionType DimensionType { get; private set; }
        [field: SerializeField] public float BaseFontSize { get; private set; }
        [field: SerializeField] public float ReferenceSize { get; private set; }
        [field: SerializeField] public bool ExpandToFitText { get; private set; }

        [UsedImplicitly]
        public event Action<string> OnTextChanged;

        private string _lastText = string.Empty;

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);

            if (executing == CanvasUpdate.PreRender && _lastText != text)
                HandleTextChanged(text);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            SetTextFont();
        }

        private void HandleTextChanged(string newText)
        {
            _lastText = newText;
            OnTextChanged?.Invoke(newText);

            ExpandContainerIfNeeded();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetTextFont();
            ExpandContainerIfNeeded();
        }

        private void SetTextFont()
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

        private void ExpandContainerIfNeeded()
        {
            if (ExpandToFitText is false || DimensionType == DimensionType.None)
                return;

            ForceMeshUpdate();

            var currentSize = rectTransform.sizeDelta;
            rectTransform.sizeDelta = DimensionType switch
            {
                DimensionType.Width => new Vector2(currentSize.x, preferredHeight),
                DimensionType.Height => new Vector2(preferredWidth, currentSize.y),
                _ => new Vector2(currentSize.x, currentSize.y)
            };
        }
    }
}