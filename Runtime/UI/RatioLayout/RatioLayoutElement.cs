using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.RatioLayout
{
    [AddComponentMenu("Layout/Ratio Layout Element")]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteInEditMode]
    public sealed class RatioLayoutElement : MonoBehaviour
#if UNITY_EDITOR
        , ILayoutSelfController
#endif
    {
        [field: SerializeField] public float Ratio { get; private set; } = 1f;

#if UNITY_EDITOR
        [SerializeField, HideIf(nameof(_isInsideRatioLayoutGroup))]
        private AspectRatioFitter.AspectMode _aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;

        [SerializeField, HideInInspector] private bool _isInsideRatioLayoutGroup;

        private RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private RectTransform _rectTransform;

        private void OnEnable()
        {
            HandleNotInsideRatioLayoutGroup();
        }
#endif

        private void OnValidate()
        {
#if UNITY_EDITOR
            HandleNotInsideRatioLayoutGroup();
#endif

            NotifyLayoutChanged();
        }

#if UNITY_EDITOR

        public void SetLayoutHorizontal()
        {
            if (_isInsideRatioLayoutGroup || _aspectMode == AspectRatioFitter.AspectMode.None)
                return;

            UpdateRect();
        }

        public void SetLayoutVertical()
        {
            if (_isInsideRatioLayoutGroup || _aspectMode == AspectRatioFitter.AspectMode.None)
                return;

            UpdateRect();
        }

        private void HandleNotInsideRatioLayoutGroup()
        {
            _isInsideRatioLayoutGroup = this.TryGetComponentInParent<RatioLayoutGroup>(out _);
            if (_isInsideRatioLayoutGroup is false && _aspectMode != AspectRatioFitter.AspectMode.None)
                SetDirty();
        }

        private void UpdateRect()
        {
            if (Ratio <= 0)
                return;

            var rect = RectTransform.rect;
            var parentRectTransform = RectTransform.parent as RectTransform;
            var parentRect = parentRectTransform ? parentRectTransform.rect : rect;

            var newSize = rect.size;

            switch (_aspectMode)
            {
                case AspectRatioFitter.AspectMode.WidthControlsHeight:
                    newSize.y = newSize.x / Ratio;
                    break;

                case AspectRatioFitter.AspectMode.HeightControlsWidth:
                    newSize.x = newSize.y * Ratio;
                    break;

                case AspectRatioFitter.AspectMode.FitInParent:
                {
                    var parentAspect = parentRect.width / parentRect.height;
                    if (Ratio > parentAspect)
                    {
                        newSize.x = parentRect.width;
                        newSize.y = newSize.x / Ratio;
                    }
                    else
                    {
                        newSize.y = parentRect.height;
                        newSize.x = newSize.y * Ratio;
                    }
                }
                    break;

                case AspectRatioFitter.AspectMode.EnvelopeParent:
                {
                    var parentAspect = parentRect.width / parentRect.height;
                    if (Ratio < parentAspect)
                    {
                        newSize.x = parentRect.width;
                        newSize.y = newSize.x / Ratio;
                    }
                    else
                    {
                        newSize.y = parentRect.height;
                        newSize.x = newSize.y * Ratio;
                    }
                }
                    break;
            }

            if (newSize == rect.size)
                return;

            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
        }

        private void SetDirty()
        {
            if (!gameObject.activeInHierarchy || !enabled)
                return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!_isInsideRatioLayoutGroup && _aspectMode != AspectRatioFitter.AspectMode.None)
                SetDirty();
        }

        private void OnDidApplyAnimationProperties()
        {
            SetDirty();
        }
#endif

        private void NotifyLayoutChanged()
        {
            if (this.TryGetComponentInParent<RatioLayoutGroup>(out var layoutGroup))
                LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
        }
    }
}