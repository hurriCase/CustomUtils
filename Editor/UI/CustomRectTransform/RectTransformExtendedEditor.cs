#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using System.Reflection;
using CustomUtils.Editor.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI.CustomRectTransform
{
    [CustomEditor(typeof(RectTransform), true)]
    internal sealed class RectTransformExtendedEditor : EditorBase
    {
        private UnityEditor.Editor _defaultEditor;
        private RectTransformRepository _repository;
        private LayoutGUIDrawer _guiDrawer;

        protected override void InitializeEditor()
        {
            _repository = new RectTransformRepository(target);
            _guiDrawer = new LayoutGUIDrawer();

            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var rectTransformEditorType = assembly.GetType("UnityEditor.RectTransformEditor");
            _defaultEditor = CreateEditor(targets, rectTransformEditorType);
        }

        protected override void CleanupEditor()
        {
            _repository?.DisposePersistentProperties();

            if (!_defaultEditor)
                return;

            DestroyImmediate(_defaultEditor);
            _defaultEditor = null;
        }

        public override void OnInspectorGUI()
        {
            _defaultEditor.OnInspectorGUI();

            serializedObject.Update();

            DrawLayoutHelper();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLayoutHelper()
        {
            EditorGUILayout.Space();

            var showFoldout = _repository.ShowProperties.Value;
            EditorVisualControls.Foldout("Layout Helper", ref showFoldout, DrawFoldoutContent);
            _repository.ShowProperties.Value = showFoldout;
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
            EditorVisualControls.Button("Recalculate from Current", () => _repository.RecalculateFromCurrent());
        }

        private void DrawSizeFields()
        {
            _guiDrawer.DrawVector2FieldStacked("Parent",
                _repository.ParentWidth,
                _repository.ParentHeight,
                EditorStateControls,
                "Width", "Height");
        }

        private void DrawMarginFields()
        {
            _guiDrawer.DrawVector2FieldHorizontal("Left/Right Margin",
                _repository.LeftMarginWidth,
                _repository.RightMarginWidth,
                EditorStateControls,
                "L", "R");

            _guiDrawer.DrawVector2FieldHorizontal("Top/Bottom Margin",
                _repository.TopMarginHeight,
                _repository.BottomMarginHeight,
                EditorStateControls,
                "T", "B");
        }

        private void DrawCalculatedContentSize()
        {
            var originalEnabled = GUI.enabled;
            GUI.enabled = false;

            _guiDrawer.DrawVector2ReadOnly("Content",
                _repository.ContentWidth,
                _repository.ContentHeight,
                "Width", "Height");

            GUI.enabled = originalEnabled;
        }

        private void ApplyAnchors()
        {
            var parentWidth = _repository.ParentWidth.Value;
            var parentHeight = _repository.ParentHeight.Value;

            if (parentWidth <= 0 || parentHeight <= 0)
                return;

            var widthRatio = 1f / parentWidth;
            var heightRatio = 1f / parentHeight;

            foreach (var selectedTarget in targets)
            {
                if (selectedTarget is not RectTransform rectTransform)
                    continue;

                Undo.RecordObject(rectTransform, "Set RectTransform Anchors and Reset Offsets");

                rectTransform.anchorMin = new Vector2(
                    widthRatio * _repository.LeftMarginWidth.Value,
                    heightRatio * _repository.BottomMarginHeight.Value);

                rectTransform.anchorMax = new Vector2(
                    1 - widthRatio * _repository.RightMarginWidth.Value,
                    1 - heightRatio * _repository.TopMarginHeight.Value);

                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                EditorUtility.SetDirty(rectTransform);
            }
        }
    }
}
#endif