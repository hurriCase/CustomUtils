#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using CustomUtils.Editor.PersistentEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CustomUtils.Editor.UI.CustomRectTransform
{
    internal sealed class RectTransformRepository
    {
        internal PersistentEditorProperty<bool> ShowProperties { get; }
        internal PersistentEditorProperty<float> ParentWidth { get; }
        internal PersistentEditorProperty<float> ParentHeight { get; }
        internal PersistentEditorProperty<float> LeftMarginWidth { get; }
        internal PersistentEditorProperty<float> RightMarginWidth { get; }
        internal PersistentEditorProperty<float> TopMarginHeight { get; }
        internal PersistentEditorProperty<float> BottomMarginHeight { get; }

        internal float ContentWidth => ParentWidth.Value - LeftMarginWidth.Value - RightMarginWidth.Value;
        internal float ContentHeight => ParentHeight.Value - TopMarginHeight.Value - BottomMarginHeight.Value;

        private readonly Object _target;
        private readonly List<PersistentEditorProperty<float>> _properties;
        private readonly LayoutCalculator _layoutCalculator;

        internal RectTransformRepository(Object target)
        {
            _target = target;
            _layoutCalculator = new LayoutCalculator();

            var layoutData = _layoutCalculator.Calculate(target);

            ShowProperties = new PersistentEditorProperty<bool>(PropertyKeys.ShowPropertiesKey, true);

            ParentWidth = target.CreatePersistentProperty(PropertyKeys.ParentWidthKey, layoutData.ParentWidth);
            ParentHeight = target.CreatePersistentProperty(PropertyKeys.ParentHeightKey, layoutData.ParentHeight);

            LeftMarginWidth = target.CreatePersistentProperty(PropertyKeys.LeftMarginWidthKey, layoutData.LeftMargin);
            TopMarginHeight = target.CreatePersistentProperty(PropertyKeys.TopMarginHeightKey, layoutData.TopMargin);
            RightMarginWidth = target.CreatePersistentProperty(PropertyKeys.RightMarginWidthKey, layoutData.RightMargin);
            BottomMarginHeight = target.CreatePersistentProperty(PropertyKeys.BottomMarginHeightKey, layoutData.BottomMargin);

            _properties = new List<PersistentEditorProperty<float>>
            {
                ParentWidth, ParentHeight, LeftMarginWidth, RightMarginWidth,
                TopMarginHeight, BottomMarginHeight
            };
        }

        internal void RecalculateFromCurrent()
        {
            var layoutData = _layoutCalculator.Calculate(_target);

            ParentWidth.Value = layoutData.ParentWidth;
            ParentHeight.Value = layoutData.ParentHeight;
            LeftMarginWidth.Value = layoutData.LeftMargin;
            RightMarginWidth.Value = layoutData.RightMargin;
            TopMarginHeight.Value = layoutData.TopMargin;
            BottomMarginHeight.Value = layoutData.BottomMargin;
        }

        internal void DisposePersistentProperties()
        {
            ShowProperties?.Dispose();
            _properties?.ForEach(prop => prop?.Dispose());
        }
    }
}
#endif