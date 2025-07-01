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
        [SerializeField] private LayoutGroup _ratioLayoutGroup;
        [SerializeField] private RectOffset _padding;

        private RectTransform RectTransform =>
            _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
        private RectTransform _rectTransform;

        private void OnRectTransformDimensionsChange()
        {
            HandleChanges();
        }

        private void Update()
        {
            HandleChanges();
        }

        private void HandleChanges()
        {
            if (_dimensionType == DimensionType.None)
                return;

            var currentSize = _dimensionType switch
            {
                DimensionType.Width => RectTransform.rect.width,
                DimensionType.Height => RectTransform.rect.height,
                _ => 0f
            };

            if (currentSize <= 0f || _parentReferenceSize <= 0f)
                return;

            var differenceRatio = _parentReferenceSize / currentSize;

            UpdateMargins(differenceRatio);
            UpdateAspectRatio(differenceRatio);
        }

        private void UpdateMargins(float differenceRatio)
        {
            if (!_ratioLayoutGroup || _dimensionType == DimensionType.None)
                return;

            switch (_dimensionType)
            {
                case DimensionType.Width:
                    _ratioLayoutGroup.padding.top = (int)(_padding.top / differenceRatio);
                    _ratioLayoutGroup.padding.bottom = (int)(_padding.bottom / differenceRatio);
                    break;

                case DimensionType.Height:
                    _ratioLayoutGroup.padding.left = (int)(_padding.left / differenceRatio);
                    _ratioLayoutGroup.padding.right = (int)(_padding.right / differenceRatio);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateAspectRatio(float differenceRatio)
        {
            if (!_referenceObject || _parentReferenceSize == 0 || _otherContentReferenceSize == 0 ||
                _dimensionType == DimensionType.None)
                return;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var (referenceObjectSize, size) = _dimensionType switch
            {
                DimensionType.Width => (_referenceObject.rect.height, RectTransform.rect.width),
                DimensionType.Height => (_referenceObject.rect.width, RectTransform.rect.height),
                _ => throw new ArgumentOutOfRangeException()
            };

            var currentOtherContentWidth = _otherContentReferenceSize / differenceRatio;
            var newRatio = (currentOtherContentWidth + referenceObjectSize) / size;

            if (newRatio <= 0)
                return;

            AspectRatioFitter.aspectRatio = newRatio;
        }
    }
}