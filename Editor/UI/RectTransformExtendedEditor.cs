#if IS_REACTTRANSFORM_EXTENDED_ENABLED
using System.Reflection;
using CustomUtils.Editor.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI
{
    [CustomEditor(typeof(RectTransform), true)]
    internal sealed class RectTransformExtendedEditor : EditorBase
    {
        private UnityEditor.Editor _defaultEditor;

        [SerializeField] private float _parentWidth;
        [SerializeField] private float _leftMarginWidth;
        [SerializeField] private float _rightMarginWidth;
        [SerializeField] private float _contentWidth;

        [SerializeField] private float _parentHeight;
        [SerializeField] private float _topMarginHeight;
        [SerializeField] private float _bottomMarginHeight;
        [SerializeField] private float _contentHeight;

        protected override void InitializeEditor()
        {
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

            DrawCustomSections();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawCustomSections()
        {
            DrawFoldoutSection("Width", DrawWidthSection);
            DrawFoldoutSection("Height", DrawHeightSection);

            if (EditorVisualControls.Button("Apply anchors") is false)
                return;

            foreach (var rectTransformTarget in targets)
            {
                var rectTransform = (RectTransform)rectTransformTarget;

                var widthAnchorRatio = 1f / _parentWidth;
                var heightAnchorRatio = 1f / _parentHeight;

                Undo.RecordObject(rectTransform, "Set RectTransform Anchors");

                rectTransform.anchorMin =
                    new Vector2(widthAnchorRatio * _leftMarginWidth, heightAnchorRatio * _bottomMarginHeight);

                rectTransform.anchorMax =
                    new Vector2(1 - widthAnchorRatio * _rightMarginWidth, 1 - heightAnchorRatio * _topMarginHeight);

                EditorUtility.SetDirty(rectTransform);
            }
        }

        private void DrawWidthSection()
        {
            _parentWidth = EditorStateControls.FloatField("Parent Width", _parentWidth);
            _leftMarginWidth = EditorStateControls.FloatField("Left Margin Width", _leftMarginWidth);
            _rightMarginWidth = EditorStateControls.FloatField("Right Margin Width", _rightMarginWidth);
            _contentWidth = EditorStateControls.FloatField("Content Width", _contentWidth);
        }

        private void DrawHeightSection()
        {
            _parentHeight = EditorStateControls.FloatField("Parent Height", _parentHeight);
            _topMarginHeight = EditorStateControls.FloatField("Top Margin Height", _topMarginHeight);
            _bottomMarginHeight = EditorStateControls.FloatField("Bottom Margin Height", _bottomMarginHeight);
            _contentHeight = EditorStateControls.FloatField("Content Height", _contentHeight);
        }
    }
}
#endif