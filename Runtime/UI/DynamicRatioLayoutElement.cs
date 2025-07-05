using System;
using CustomUtils.Runtime.CustomBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AspectRatioFitter))]
    internal sealed class DynamicRatioLayoutElement : AspectRatioBehaviour
    {
        [SerializeField] private RectTransform _referenceObject;
        [SerializeField] private DimensionType _dimensionType;
        [SerializeField] private float _parentReferenceSize;
        [SerializeField] private float _otherContentReferenceSize;

        private RectTransform RectTransform =>
            _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
        private RectTransform _rectTransform;

        private void OnRectTransformDimensionsChange()
        {
            UpdateAspectRatio();
        }

        private void OnValidate()
        {
            UpdateAspectRatio();
        }

        private void UpdateAspectRatio()
        {
            if (!_referenceObject || _parentReferenceSize <= 0 || _otherContentReferenceSize <= 0 ||
                _dimensionType == DimensionType.None)
                return;

            var currentSize = _dimensionType switch
            {
                DimensionType.Width => RectTransform.rect.width,
                DimensionType.Height => RectTransform.rect.height,
                _ => 0f
            };

            if (currentSize <= 0f)
                return;

            var (referenceObjectSize, size) = _dimensionType switch
            {
                DimensionType.Width => (_referenceObject.rect.height, currentSize),
                DimensionType.Height => (_referenceObject.rect.width, currentSize),
                _ => throw new ArgumentOutOfRangeException()
            };

            var scaledOtherContentSize = _otherContentReferenceSize * currentSize / _parentReferenceSize;
            var newRatio = (scaledOtherContentSize + referenceObjectSize) / size;

            if (newRatio <= 0)
                return;

            AspectRatioFitter.aspectRatio = newRatio;
        }
    }
}