using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.RatioLayout
{
    [AddComponentMenu("Layout/Ratio Layout Group")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class RatioLayoutGroup : LayoutGroup
    {
        [SerializeField] private LayoutDirection _direction = LayoutDirection.Horizontal;
        [SerializeField] private float _spacing;
        [HideInInspector, SerializeField] private List<RatioLayoutElement> _ratioElements = new();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            RefreshRatioElements();
            CalculateLayoutSizes();
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateLayoutSizes();
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            RefreshRatioElements();
        }

        private void RefreshRatioElements()
        {
            _ratioElements.Clear();

            foreach (var child in rectChildren)
            {
                if (child.TryGetComponent<RatioLayoutElement>(out var ratioElement))
                    _ratioElements.Add(ratioElement);
            }
        }

        private void CalculateLayoutSizes()
        {
            if (_ratioElements.Count == 0)
                return;

            var rect = rectTransform.rect;
            var availableWidth = rect.width - padding.horizontal;
            var availableHeight = rect.height - padding.vertical;
            var spacingTotal = _spacing * Mathf.Max(0, _ratioElements.Count - 1);

            var totalRequiredSize = 0f;

            if (_direction == LayoutDirection.Horizontal)
            {
                totalRequiredSize += _ratioElements.Sum(element => availableHeight * element.Ratio);

                totalRequiredSize += spacingTotal;

                SetLayoutInputForAxis(totalRequiredSize, totalRequiredSize, -1, 0);
                SetLayoutInputForAxis(availableHeight, availableHeight, -1, 1);
            }
            else
            {
                var childWidth = availableWidth;
                totalRequiredSize += _ratioElements.Sum(element => childWidth / element.Ratio);

                totalRequiredSize += spacingTotal;

                SetLayoutInputForAxis(availableWidth, availableWidth, -1, 0);
                SetLayoutInputForAxis(totalRequiredSize, totalRequiredSize, -1, 1);
            }
        }

        private void SetChildrenAlongAxis(int axis)
        {
            var rect = rectTransform.rect;
            var availableWidth = rect.width - padding.horizontal;
            var availableHeight = rect.height - padding.vertical;

            var position = axis == 0 ? (float)padding.left : padding.bottom;

            for (var i = 0; i < rectChildren.Count && i < _ratioElements.Count; i++)
            {
                var child = rectChildren[i];
                var ratio = _ratioElements[i].Ratio;

                float size;
                if (_direction == LayoutDirection.Horizontal)
                {
                    if (axis == 0)
                        size = availableHeight * ratio;
                    else
                        size = availableHeight;
                }
                else
                {
                    if (axis == 0)
                        size = availableWidth;
                    else
                        size = availableWidth / ratio;
                }

                SetChildAlongAxis(child, axis, position, size);

                if ((_direction == LayoutDirection.Horizontal && axis == 0) ||
                    (_direction == LayoutDirection.Vertical && axis == 1))
                    position += size + _spacing;
            }
        }
    }
}