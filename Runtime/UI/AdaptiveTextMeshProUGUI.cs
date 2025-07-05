using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    public sealed class AdaptiveTextMeshProUGUI : TextMeshProUGUI, ILayoutSelfController
    {
        [field: SerializeField] public DimensionType DimensionType { get; private set; }
        [field: SerializeField] public float BaseFontSize { get; private set; }
        [field: SerializeField] public float ReferenceSize { get; private set; }
        [field: SerializeField] public bool ExpandToFitText { get; private set; }

        [UsedImplicitly]
        public event Action<string> OnTextChanged;

        private DrivenRectTransformTracker _tracker;
        private string _lastText = string.Empty;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            _tracker.Clear();
            base.OnDisable();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SetTextFont();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            SetTextFont();
            ExpandContainerIfNeeded();
        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);

            if (executing == CanvasUpdate.PreRender && _lastText != text)
                HandleTextChanged(text);
        }

        public void SetLayoutHorizontal() { }
        public void SetLayoutVertical() { }

        private void SetDirty()
        {
            if (IsActive() is false)
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        private void HandleTextChanged(string newText)
        {
            _lastText = newText;

            OnTextChanged?.Invoke(newText);

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
            {
                _tracker.Clear();
                return;
            }

            ForceMeshUpdate();

            switch (DimensionType)
            {
                case DimensionType.Width:
                    _tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
                    break;

                case DimensionType.Height:
                    _tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
                    break;
            }
        }
    }
}