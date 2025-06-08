using System;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.RatioLayout
{
    /// <summary>
    /// Resizes a RectTransform to fit a specified aspect ratio when not inside a RatioLayoutGroup.
    /// When inside a RatioLayoutGroup, acts as a data provider for the layout group.
    /// </summary>
    [AddComponentMenu("Layout/Ratio Layout Element")]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class RatioLayoutElement : UIBehaviour
#if UNITY_EDITOR
        , ILayoutSelfController
#endif
    {
        [SerializeField] private float _aspectRatio = 1;

        /// <summary>
        /// The aspect ratio to enforce. This means width divided by height.
        /// </summary>
        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                if (SetPropertyUtility.TrySetStruct(ref _aspectRatio, value))
#if UNITY_EDITOR
                    SetDirty();
#else
                    NotifyLayoutChanged();
#endif
            }
        }

        private void NotifyLayoutChanged()
        {
            if (this.TryGetComponentInParent<RatioLayoutGroup>(out var layoutGroup))
                LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
        }

#if UNITY_EDITOR

        // Simulating AspectRatioFitter logic when we not inside RatioLayoutGroup to simplify a
        // prefab configuration process, and it isn't used in Runtime.
        // Unfortunately, AspectRatioFitter doesn't provide the way to fully alter logic to make the same job through inheritance

        [SerializeField, HideIf(nameof(_isInsideRatioLayoutGroup))]
        private AspectRatioFitter.AspectMode _aspectMode = AspectRatioFitter.AspectMode.None;

        /// <summary>
        /// The mode to use to enforce the aspect ratio.
        /// </summary>
        public AspectRatioFitter.AspectMode AspectMode
        {
            get => _aspectMode;
            set
            {
                if (SetPropertyUtility.TrySetStruct(ref _aspectMode, value)) SetDirty();
            }
        }

        [NonSerialized] private RectTransform _rectTransform;

        private bool _delayedSetDirty;

        private bool _doesParentExist;

        [SerializeField, HideInInspector] private bool _isInsideRatioLayoutGroup;

        private RectTransform RectTransformTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private DrivenRectTransformTracker _tracker;

        protected RatioLayoutElement() { }

        protected override void OnEnable()
        {
            base.OnEnable();

            _doesParentExist = RectTransformTransform.parent;

            UpdateLayoutGroupStatus();

            SetDirty();
        }

        protected override void Start()
        {
            base.Start();

            if (IsComponentValidOnObject() is false || IsAspectModeValid() is false)
                enabled = false;
        }

        protected override void OnDisable()
        {
            _tracker.Clear();

            LayoutRebuilder.MarkLayoutForRebuild(RectTransformTransform);

            base.OnDisable();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            _doesParentExist = RectTransformTransform.parent;

            UpdateLayoutGroupStatus();

            SetDirty();
        }

        protected virtual void Update()
        {
            if (_delayedSetDirty is false)
                return;

            _delayedSetDirty = false;
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateLayoutGroupStatus()
        {
            var wasInsideLayoutGroup = _isInsideRatioLayoutGroup;
            _isInsideRatioLayoutGroup = this.TryGetComponentInParent<RatioLayoutGroup>(out _);

            if (wasInsideLayoutGroup == _isInsideRatioLayoutGroup)
                return;

            NotifyLayoutChanged();
        }

        private void UpdateRect()
        {
            if (IsActive() is false || IsComponentValidOnObject() is false)
                return;

            if (_isInsideRatioLayoutGroup)
            {
                _tracker.Clear();
                return;
            }

            _tracker.Clear();

            switch (_aspectMode)
            {
                case AspectRatioFitter.AspectMode.None:
                {
                    var rect = RectTransformTransform.rect;
                    _aspectRatio = Mathf.Clamp(rect.width / rect.height, 0.001f,
                        1000f);

                    break;
                }

                case AspectRatioFitter.AspectMode.HeightControlsWidth:
                {
                    _tracker.Add(this, RectTransformTransform, DrivenTransformProperties.SizeDeltaX);
                    RectTransformTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        RectTransformTransform.rect.height * _aspectRatio);
                    break;
                }

                case AspectRatioFitter.AspectMode.WidthControlsHeight:
                {
                    _tracker.Add(this, RectTransformTransform, DrivenTransformProperties.SizeDeltaY);
                    RectTransformTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        RectTransformTransform.rect.width / _aspectRatio);
                    break;
                }

                case AspectRatioFitter.AspectMode.FitInParent:
                case AspectRatioFitter.AspectMode.EnvelopeParent:
                {
                    if (DoesParentExists() is false)
                        break;

                    _tracker.Add(this, RectTransformTransform,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDeltaX |
                        DrivenTransformProperties.SizeDeltaY);

                    RectTransformTransform.anchorMin = Vector2.zero;
                    RectTransformTransform.anchorMax = Vector2.one;
                    RectTransformTransform.anchoredPosition = Vector2.zero;

                    var sizeDelta = Vector2.zero;
                    var parentSize = GetParentSize();
                    if ((parentSize.y * AspectRatio < parentSize.x) ^
                        (_aspectMode == AspectRatioFitter.AspectMode.FitInParent))
                        sizeDelta.y = GetSizeDeltaToProduceSize(parentSize.x / AspectRatio, 1);
                    else
                        sizeDelta.x = GetSizeDeltaToProduceSize(parentSize.y * AspectRatio, 0);

                    RectTransformTransform.sizeDelta = sizeDelta;

                    break;
                }
            }
        }

        private float GetSizeDeltaToProduceSize(float size, int axis) => size - GetParentSize()[axis] *
            (RectTransformTransform.anchorMax[axis] - RectTransformTransform.anchorMin[axis]);

        private Vector2 GetParentSize()
        {
            var parent = RectTransformTransform.parent as RectTransform;
            return !parent ? Vector2.zero : parent.rect.size;
        }

        public virtual void SetLayoutHorizontal()
        {
            if (_isInsideRatioLayoutGroup is false)
                UpdateRect();
        }

        public virtual void SetLayoutVertical()
        {
            if (_isInsideRatioLayoutGroup is false)
                UpdateRect();
        }

        private void SetDirty()
        {
            if (IsActive() is false)
                return;

            if (_isInsideRatioLayoutGroup)
                NotifyLayoutChanged();
            else
                UpdateRect();
        }

        private bool IsComponentValidOnObject()
        {
            var canvas = gameObject.GetComponent<Canvas>();
            return !canvas || !canvas.isRootCanvas || canvas.renderMode == RenderMode.WorldSpace;
        }

        private bool IsAspectModeValid() =>
            DoesParentExists() ||
            (AspectMode != AspectRatioFitter.AspectMode.EnvelopeParent
             && AspectMode != AspectRatioFitter.AspectMode.FitInParent);

        private bool DoesParentExists() => _doesParentExist;

        protected override void OnValidate()
        {
            _aspectRatio = Mathf.Clamp(_aspectRatio, 0.001f, 1000f);
            _delayedSetDirty = true;

            UpdateLayoutGroupStatus();
        }
#endif
    }
}