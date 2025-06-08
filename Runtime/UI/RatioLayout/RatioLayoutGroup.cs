using System.Collections.Generic;
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
        [SerializeField] private bool _reverseArrangement;
        [Space]
        [SerializeField] private bool _childForceExpandWidth;
        [SerializeField] private bool _childForceExpandHeight;
        [Space]
        [SerializeField] private bool _childControlWidth;
        [SerializeField] private bool _childControlHeight;
        [Space]
        [SerializeField] private bool _childScaleWidth;
        [SerializeField] private bool _childScaleHeight;

        [HideInInspector, SerializeField] private List<RatioLayoutElement> _ratioElements = new();

        private readonly List<ChildInfo> _childInfos = new();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            RefreshChildInfos();

            CalcAlongAxis(0, _direction == LayoutDirection.Vertical);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, _direction == LayoutDirection.Vertical);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, _direction == LayoutDirection.Vertical);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, _direction == LayoutDirection.Vertical);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshChildInfos();
        }

        private void RefreshChildInfos()
        {
            _ratioElements.Clear();
            _childInfos.Clear();

            foreach (var child in rectChildren)
            {
                var childInfo = new ChildInfo
                {
                    RectTransform = child,
                    RatioElement = child.GetComponent<RatioLayoutElement>(),
                    IsRatio = false
                };

                if (childInfo.RatioElement)
                {
                    childInfo.IsRatio = true;
                    _ratioElements.Add(childInfo.RatioElement);
                }

                _childInfos.Add(childInfo);
            }
        }

        private void CalcAlongAxis(int axis, bool isVertical)
        {
            float combinedPadding = axis == 0 ? padding.horizontal : padding.vertical;
            var controlSize = axis == 0 ? _childControlWidth : _childControlHeight;
            var useScale = axis == 0 ? _childScaleWidth : _childScaleHeight;
            var childForceExpandSize = axis == 0 ? _childForceExpandWidth : _childForceExpandHeight;

            var totalMin = combinedPadding;
            var totalPreferred = combinedPadding;
            float totalFlexible = 0;

            var alongOtherAxis = isVertical ^ (axis == 1);

            foreach (var childInfo in _childInfos)
            {
                float min, preferred, flexible;

                if (childInfo.IsRatio)
                    GetRatioChildSizes(childInfo.RatioElement, axis, controlSize, childForceExpandSize, out min,
                        out preferred, out flexible);
                else
                    GetChildSizes(childInfo.RectTransform, axis, controlSize, childForceExpandSize, out min,
                        out preferred, out flexible);

                if (useScale)
                {
                    var scaleFactor = childInfo.RectTransform.localScale[axis];
                    min *= scaleFactor;
                    preferred *= scaleFactor;
                    flexible *= scaleFactor;
                }

                if (alongOtherAxis)
                {
                    totalMin = Mathf.Max(min + combinedPadding, totalMin);
                    totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
                    totalFlexible = Mathf.Max(flexible, totalFlexible);
                }
                else
                {
                    totalMin += min + _spacing;
                    totalPreferred += preferred + _spacing;
                    totalFlexible += flexible;
                }
            }

            if (!alongOtherAxis && _childInfos.Count > 0)
            {
                totalMin -= _spacing;
                totalPreferred -= _spacing;
            }

            totalPreferred = Mathf.Max(totalMin, totalPreferred);
            SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
        }

        private void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            var size = rectTransform.rect.size[axis];
            var controlSize = axis == 0 ? _childControlWidth : _childControlHeight;
            var useScale = axis == 0 ? _childScaleWidth : _childScaleHeight;
            var childForceExpandSize = axis == 0 ? _childForceExpandWidth : _childForceExpandHeight;
            var alignmentOnAxis = GetAlignmentOnAxis(axis);

            var alongOtherAxis = isVertical ^ (axis == 1);
            var startIndex = _reverseArrangement ? _childInfos.Count - 1 : 0;
            var endIndex = _reverseArrangement ? 0 : _childInfos.Count;
            var increment = _reverseArrangement ? -1 : 1;

            if (alongOtherAxis)
            {
                var innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);

                for (var i = startIndex; _reverseArrangement ? i >= endIndex : i < endIndex; i += increment)
                {
                    var childInfo = _childInfos[i];
                    float min, preferred, flexible;

                    if (childInfo.IsRatio)
                        GetRatioChildSizes(childInfo.RatioElement, axis, controlSize, childForceExpandSize, out min,
                            out preferred, out flexible);
                    else
                        GetChildSizes(childInfo.RectTransform, axis, controlSize, childForceExpandSize, out min,
                            out preferred, out flexible);

                    var scaleFactor = useScale ? childInfo.RectTransform.localScale[axis] : 1f;

                    var requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                    var startOffset = GetStartOffset(axis, requiredSpace * scaleFactor);

                    if (controlSize)
                    {
                        if (childInfo.IsRatio)
                            SetChildAlongAxisWithRatio(childInfo, axis, startOffset, requiredSpace, scaleFactor);
                        else
                            SetChildAlongAxisWithScale(childInfo.RectTransform, axis, startOffset, requiredSpace,
                                scaleFactor);
                    }
                    else
                    {
                        var offsetInCell = (requiredSpace - childInfo.RectTransform.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScale(childInfo.RectTransform, axis, startOffset + offsetInCell,
                            scaleFactor);
                    }
                }
            }
            else
            {
                float pos = axis == 0 ? padding.left : padding.top;
                float itemFlexibleMultiplier = 0;
                var surplusSpace = size - GetTotalPreferredSize(axis);

                if (surplusSpace > 0)
                {
                    if (GetTotalFlexibleSize(axis) == 0)
                        pos = GetStartOffset(axis,
                            GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));
                    else if (GetTotalFlexibleSize(axis) > 0)
                        itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
                }

                float minMaxLerp = 0;
                if (Mathf.Approximately(GetTotalMinSize(axis), GetTotalPreferredSize(axis)) is false)
                    minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) /
                                               (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

                for (var i = startIndex; _reverseArrangement ? i >= endIndex : i < endIndex; i += increment)
                {
                    var childInfo = _childInfos[i];
                    float min, preferred, flexible;

                    if (childInfo.IsRatio)
                        GetRatioChildSizes(childInfo.RatioElement, axis, controlSize, childForceExpandSize, out min,
                            out preferred, out flexible);
                    else
                        GetChildSizes(childInfo.RectTransform, axis, controlSize, childForceExpandSize, out min,
                            out preferred, out flexible);

                    var scaleFactor = useScale ? childInfo.RectTransform.localScale[axis] : 1f;

                    var childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                    childSize += flexible * itemFlexibleMultiplier;

                    if (controlSize)
                    {
                        if (childInfo.IsRatio)
                            SetChildAlongAxisWithRatio(childInfo, axis, pos, childSize, scaleFactor);
                        else
                            SetChildAlongAxisWithScale(childInfo.RectTransform, axis, pos, childSize, scaleFactor);
                    }
                    else
                    {
                        var offsetInCell = (childSize - childInfo.RectTransform.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScale(childInfo.RectTransform, axis, pos + offsetInCell, scaleFactor);
                    }

                    pos += childSize * scaleFactor + _spacing;
                }
            }
        }

        private void SetChildAlongAxisWithRatio(ChildInfo childInfo, int axis, float pos, float size, float scaleFactor)
        {
            var rect = childInfo.RectTransform;

            m_Tracker.Add(this, rect,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.AnchoredPosition |
                DrivenTransformProperties.SizeDeltaX |
                DrivenTransformProperties.SizeDeltaY);

            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            var sizeDelta = rect.sizeDelta;

            if (_direction == LayoutDirection.Horizontal)
            {
                if (axis == 1)
                {
                    sizeDelta.y = size;
                    sizeDelta.x = size * childInfo.RatioElement.AspectRatio;
                }
                else
                    sizeDelta.x = size;
            }
            else
            {
                if (axis == 0)
                {
                    sizeDelta.x = size;
                    sizeDelta.y = size / childInfo.RatioElement.AspectRatio;
                }
                else
                    sizeDelta.y = size;
            }

            rect.sizeDelta = sizeDelta;

            var anchoredPosition = rect.anchoredPosition;
            anchoredPosition[axis] = axis == 0
                ? pos + sizeDelta[axis] * rect.pivot[axis] * scaleFactor
                : -pos - sizeDelta[axis] * (1f - rect.pivot[axis]) * scaleFactor;
            rect.anchoredPosition = anchoredPosition;
        }

        private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand,
            out float min, out float preferred, out float flexible)
        {
            if (controlSize is false)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }

        private void GetRatioChildSizes(RatioLayoutElement ratioElement, int axis, bool controlSize,
            bool childForceExpand,
            out float min, out float preferred, out float flexible)
        {
            var rect = rectTransform.rect;
            var availableWidth = rect.width - padding.horizontal;
            var availableHeight = rect.height - padding.vertical;

            if (_direction == LayoutDirection.Horizontal)
            {
                if (axis == 0)
                {
                    min = preferred = availableHeight * ratioElement.AspectRatio;
                    flexible = 0;
                }
                else
                {
                    min = preferred = availableHeight;
                    flexible = childForceExpand ? 1 : 0;
                }
            }
            else
            {
                if (axis == 0)
                {
                    min = preferred = availableWidth;
                    flexible = childForceExpand ? 1 : 0;
                }
                else
                {
                    min = preferred = availableWidth / ratioElement.AspectRatio;
                    flexible = 0;
                }
            }

            if (controlSize)
                return;

            var child = ratioElement.RectTransform;
            min = child.sizeDelta[axis];
            preferred = min;
            flexible = 0;
        }
    }
}