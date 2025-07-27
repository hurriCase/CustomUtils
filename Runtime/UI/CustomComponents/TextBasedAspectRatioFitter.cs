﻿using CustomUtils.Runtime.CustomBehaviours;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AspectRatioFitter))]
    internal sealed class TextBasedAspectRatioFitter : AspectRatioBehaviour, ITextSizeNotifiable
    {
        [SerializeField] private AdaptiveTextMeshProUGUI[] _adaptiveTexts;
        [SerializeField] private DimensionType _staticDimensionType;
        [SerializeField] private float _staticReferenceSize;
        [SerializeField] private float _otherContentReferenceSize;

        private RectTransform RectTransform =>
            _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
        private RectTransform _rectTransform;

        private bool _isRegistered;

        private void OnEnable()
        {
            ChangeRegistrationState(true);
        }

        private void OnDisable()
        {
            ChangeRegistrationState(false);
        }

        private void OnValidate()
        {
            ChangeRegistrationState(true);
        }

        private void ChangeRegistrationState(bool newState)
        {
            if (_adaptiveTexts == null || _adaptiveTexts.Length == 0 || _isRegistered == newState)
                return;

            foreach (var adaptiveText in _adaptiveTexts)
            {
                if (!adaptiveText)
                    continue;

                if (newState)
                {
                    adaptiveText.RegisterSizeNotifiable(this);
                    continue;
                }

                adaptiveText.UnregisterSizeNotifiable(this);
            }

            _isRegistered = newState;
        }

        public void OnTextSizeChanged()
        {
            UpdateAspectRatio();
        }

        private void UpdateAspectRatio()
        {
            if (Validate() is false)
                return;

            var totalTextSize = CalculateTotalTextSize();

            if (TryGetNewRatio(totalTextSize, out var newRatio) is false)
                return;

            AspectRatioFitter.aspectRatio = newRatio;
        }

        private bool TryGetNewRatio(float totalTextSize, out float newRatio)
        {
            newRatio = 0f;

            var currentSize = _staticDimensionType switch
            {
                DimensionType.Width => RectTransform.rect.width,
                DimensionType.Height => RectTransform.rect.height,
                _ => 0f
            };

            if (currentSize.IsReasonable() is false)
                return false;

            var scaledOtherContentSize = _otherContentReferenceSize * (currentSize / _staticReferenceSize);
            var targetDynamicSize = scaledOtherContentSize + totalTextSize;

            newRatio = _staticDimensionType switch
            {
                DimensionType.Width => currentSize / targetDynamicSize,
                DimensionType.Height => targetDynamicSize / currentSize,
                _ => 1f
            };

            return newRatio.IsReasonable();
        }

        private float CalculateTotalTextSize()
        {
            var totalTextSize = 0f;

            foreach (var adaptiveText in _adaptiveTexts)
            {
                if (!adaptiveText)
                    continue;

                totalTextSize += _staticDimensionType switch
                {
                    DimensionType.Width => adaptiveText.rectTransform.rect.height,
                    DimensionType.Height => adaptiveText.rectTransform.rect.width,
                    _ => 0f
                };
            }

            return totalTextSize;
        }

        private bool Validate() =>
            _adaptiveTexts is { Length: > 0 } &&
            _staticReferenceSize > 0 && _otherContentReferenceSize > 0 &&
            _staticDimensionType != DimensionType.None;
    }
}