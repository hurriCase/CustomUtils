using System;
using JetBrains.Annotations;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    public sealed class AdaptiveTextMeshProUGUI : TextMeshProUGUI, ILayoutSelfController, ITextPreprocessor
    {
        [field: SerializeField] public DimensionType DimensionType { get; private set; }
        [field: SerializeField] public float BaseFontSize { get; private set; }
        [field: SerializeField] public float ReferenceSize { get; private set; }
        [field: SerializeField] public bool ExpandToFitText { get; private set; }

        private readonly Subject<string> _textChangedSubject = new();

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

        protected override void OnDestroy()
        {
            _textChangedSubject?.Dispose();

            base.OnDestroy();
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

        public string PreprocessText(string text)
        {
            if (_lastText == text)
                return text;

            _lastText = text;
            _textChangedSubject.OnNext(text);
            ExpandContainerIfNeeded();

            return text;
        }

        /// <summary>
        /// Subscribes to value changes
        /// </summary>
        /// <param name="target">Target object to pass to the callback</param>
        /// <param name="onNext">Action to execute when value changes</param>
        /// <returns>Disposable subscription</returns>
        [UsedImplicitly]
        public IDisposable Subscribe<TTarget>(TTarget target, Action<string, TTarget> onNext)
        {
            return _textChangedSubject.Subscribe(
                (target, onNext),
                static (property, tuple) => tuple.onNext(property, tuple.target));
        }

        public void SetLayoutHorizontal() { }

        public void SetLayoutVertical() { }

        private void SetDirty()
        {
            if (IsActive() is false)
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
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