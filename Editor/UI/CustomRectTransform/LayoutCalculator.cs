#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Editor.UI.CustomRectTransform
{
    internal sealed class LayoutCalculator
    {
        internal LayoutData Calculate(Object target)
        {
            var rectTransform = target as RectTransform;
            if (!rectTransform)
                return LayoutData.Empty;

            var parentSize = CalculateParentSize(rectTransform);
            var margins = CalculateMargins(rectTransform, parentSize);

            return new LayoutData
            {
                ParentWidth = parentSize.x,
                ParentHeight = parentSize.y,
                LeftMargin = margins.Left,
                RightMargin = margins.Right,
                TopMargin = margins.Top,
                BottomMargin = margins.Bottom
            };
        }

        private Vector2 CalculateParentSize(Transform rectTransform)
        {
            var parentRectTransform = rectTransform.parent as RectTransform;
            if (!parentRectTransform)
                return GetCanvasSize(rectTransform);

            var parentRect = parentRectTransform.rect;
            return new Vector2(Mathf.Abs(parentRect.width), Mathf.Abs(parentRect.height));
        }

        private Vector2 GetCanvasSize(Component rectTransform)
        {
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (!canvas) return Vector2.zero;

            var canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect)
                return new Vector2(Mathf.Abs(canvasRect.rect.width), Mathf.Abs(canvasRect.rect.height));

            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            if (canvasScaler && canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                return canvasScaler.referenceResolution;

            return Vector2.zero;
        }

        private Margins CalculateMargins(RectTransform rectTransform, Vector2 parentSize)
        {
            if (parentSize.x <= 0 || parentSize.y <= 0)
                return Margins.Zero;

            var horizontalMargins = CalculateHorizontalMargins(rectTransform, parentSize.x);
            var verticalMargins = CalculateVerticalMargins(rectTransform, parentSize.y);

            return new Margins
            {
                Left = Mathf.Max(0, horizontalMargins.left),
                Right = Mathf.Max(0, horizontalMargins.right),
                Top = Mathf.Max(0, verticalMargins.top),
                Bottom = Mathf.Max(0, verticalMargins.bottom)
            };
        }

        private (float left, float right) CalculateHorizontalMargins(RectTransform rectTransform, float parentWidth)
        {
            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;

            if (Mathf.Approximately(anchorMin.x, anchorMax.x))
            {
                var anchorX = anchorMin.x * parentWidth;
                var pivot = rectTransform.pivot;
                var sizeDelta = rectTransform.sizeDelta;
                var anchoredPosition = rectTransform.anchoredPosition;

                var left = anchorX + anchoredPosition.x - sizeDelta.x * pivot.x;
                var right = parentWidth - (anchorX + anchoredPosition.x + sizeDelta.x * (1 - pivot.x));
                return (left, right);
            }
            else
            {
                var left = anchorMin.x * parentWidth;
                var right = (1 - anchorMax.x) * parentWidth;
                return (left, right);
            }
        }

        private (float top, float bottom) CalculateVerticalMargins(RectTransform rectTransform, float parentHeight)
        {
            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;

            if (Mathf.Approximately(anchorMin.y, anchorMax.y))
            {
                var anchorY = anchorMin.y * parentHeight;
                var pivot = rectTransform.pivot;
                var sizeDelta = rectTransform.sizeDelta;
                var anchoredPosition = rectTransform.anchoredPosition;

                var bottom = anchorY + anchoredPosition.y - sizeDelta.y * pivot.y;
                var top = parentHeight - (anchorY + anchoredPosition.y + sizeDelta.y * (1 - pivot.y));
                return (top, bottom);
            }
            else
            {
                var bottom = anchorMin.y * parentHeight;
                var top = (1 - anchorMax.y) * parentHeight;
                return (top, bottom);
            }
        }
    }
}
#endif