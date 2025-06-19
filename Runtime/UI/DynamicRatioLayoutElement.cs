using System;
using CustomUtils.Runtime.CustomBehaviours;
using CustomUtils.Runtime.UI.RatioLayout;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(RatioLayoutElement))]
    internal sealed class DynamicRatioLayoutElement : RatioLayoutBehaviour
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

            var differenceRatio = _dimensionType switch
            {
                DimensionType.Width => _parentReferenceSize / RectTransform.rect.width,
                DimensionType.Height => _parentReferenceSize / RectTransform.rect.height
            };

            UpdateMargins(differenceRatio);
            UpdateAspectRatio(differenceRatio);
        }

        private void UpdateMargins(float differenceRatio)
        {
            if (!_ratioLayoutGroup)
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
            if (!_referenceObject || _parentReferenceSize == 0 || _otherContentReferenceSize == 0)
                return;

            var (referenceObjectSize, size) = _dimensionType switch
            {
                DimensionType.Width => (_referenceObject.rect.height, RectTransform.rect.width),
                DimensionType.Height => (_referenceObject.rect.width, RectTransform.rect.height)
            };

            var currentOtherContentWidth = _otherContentReferenceSize / differenceRatio;
            var newRatio = (currentOtherContentWidth + referenceObjectSize) / size;

            if (newRatio <= 0)
                return;

            RatioLayoutElement.AspectRatio = newRatio;
        }
    }
}