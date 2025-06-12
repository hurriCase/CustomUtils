#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using System.Reflection;
using CustomUtils.Editor.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI.CustomRectTransform
{
    [CustomEditor(typeof(RectTransform), true)]
    [CanEditMultipleObjects]
    internal sealed class RectTransformExtendedEditor : EditorBase
    {
        private const string ShowLayoutHelperPref = "RectTransformExtended.ShowLayoutHelper";

        private UnityEditor.Editor _defaultEditor;
        private LayoutGUIDrawer _guiDrawer;
        private LayoutCalculator _layoutCalculator;
        private bool _hasInitialized;

        private float _parentWidth;
        private float _parentHeight;
        private float _leftMargin;
        private float _rightMargin;
        private float _topMargin;
        private float _bottomMargin;

        protected override void InitializeEditor()
        {
            _guiDrawer = new LayoutGUIDrawer();
            _layoutCalculator = new LayoutCalculator();
            _hasInitialized = false;

            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var rectTransformEditorType = assembly.GetType("UnityEditor.RectTransformEditor");
            _defaultEditor = CreateEditor(targets, rectTransformEditorType);
        }

        protected override void CleanupEditor()
        {
            if (!_defaultEditor)
                return;

            DestroyImmediate(_defaultEditor);
            _defaultEditor = null;
        }

        public override void OnInspectorGUI()
        {
            _defaultEditor.OnInspectorGUI();

            serializedObject.Update();

            if (!_hasInitialized)
            {
                RecalculateFromCurrent();
                _hasInitialized = true;
            }

            DrawLayoutHelper();

            serializedObject.ApplyModifiedProperties();
        }

        private void RecalculateFromCurrent()
        {
            var layoutData = _layoutCalculator.Calculate(target);
            _parentWidth = layoutData.ParentWidth;
            _parentHeight = layoutData.ParentHeight;
            _leftMargin = layoutData.LeftMargin;
            _rightMargin = layoutData.RightMargin;
            _topMargin = layoutData.TopMargin;
            _bottomMargin = layoutData.BottomMargin;
        }

        private void DrawLayoutHelper()
        {
            EditorGUILayout.Space();

            var showFoldout = EditorPrefs.GetBool(ShowLayoutHelperPref, true);
            EditorVisualControls.Foldout("Layout Helper", ref showFoldout, DrawFoldoutContent);
            EditorPrefs.SetBool(ShowLayoutHelperPref, showFoldout);
        }

        private void DrawFoldoutContent()
        {
            DrawSizeFields();
            EditorGUILayout.Space();

            DrawMarginFields();
            EditorGUILayout.Space();

            DrawCalculatedContentSize();
            EditorGUILayout.Space();

            DrawButtonsSection();
        }

        private void DrawButtonsSection()
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorVisualControls.Button("Apply Anchors", ApplyAnchors);
            EditorVisualControls.Button("Recalculate from Current", RecalculateFromCurrent);
        }

        private void DrawSizeFields()
        {
            _guiDrawer.DrawVector2FieldStacked("Parent", ref _parentWidth,
                ref _parentHeight, EditorStateControls, "Width", "Height");
        }

        private void DrawMarginFields()
        {
            _guiDrawer.DrawVector2FieldHorizontal("Left/Right Margin", ref _leftMargin, ref _rightMargin,
                EditorStateControls, "L", "R");
            _guiDrawer.DrawVector2FieldHorizontal("Top/Bottom Margin", ref _topMargin, ref _bottomMargin,
                EditorStateControls, "T", "B");
        }

        private void DrawCalculatedContentSize()
        {
            var contentWidth = _parentWidth - _leftMargin - _rightMargin;
            var contentHeight = _parentHeight - _topMargin - _bottomMargin;

            _guiDrawer.DrawVector2ReadOnly("Content",
                contentWidth,
                contentHeight,
                "Width", "Height");
        }

        private void ApplyAnchors()
        {
            if (_parentWidth <= 0 || _parentHeight <= 0)
                return;

            var widthRatio = 1f / _parentWidth;
            var heightRatio = 1f / _parentHeight;

            foreach (var selectedTarget in targets)
            {
                if (selectedTarget is not RectTransform rectTransform)
                    continue;

                Undo.RecordObject(rectTransform, "Set RectTransform Anchors and Reset Offsets");

                rectTransform.anchorMin = new Vector2(
                    widthRatio * _leftMargin,
                    heightRatio * _bottomMargin);

                rectTransform.anchorMax = new Vector2(
                    1 - widthRatio * _rightMargin,
                    1 - heightRatio * _topMargin);

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                EditorUtility.SetDirty(rectTransform);
            }
        }
    }
}
#endif