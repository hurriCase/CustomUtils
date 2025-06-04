#if IS_RECTTRANSFORM_EXTENDED_ENABLED
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
        [SerializeField] private float _parentHeight;

        [SerializeField] private float _leftMarginWidth;
        [SerializeField] private float _rightMarginWidth;
        [SerializeField] private float _topMarginHeight;
        [SerializeField] private float _bottomMarginHeight;

        [SerializeField] private float _contentWidth;
        [SerializeField] private float _contentHeight;

        [SerializeField] private bool _showCustomProperties = false;

        private static readonly MethodInfo _multiFieldPrefixLabelMethod;
        private static readonly MethodInfo _calcPrefixLabelWidthMethod;

        protected override void InitializeEditor()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var rectTransformEditorType = assembly.GetType("UnityEditor.RectTransformEditor");

            _defaultEditor = CreateEditor(targets, rectTransformEditorType);

            _showCustomProperties = EditorPrefs.GetBool("RectTransformExtended.showLayoutHelper", false);
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

            EditorGUILayout.Space();

            DrawCustomSections();

            serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawCustomSections()
        {
            var foldoutStyle = EditorVisualControls.CreateTextStyle(
                EditorStyles.foldout,
                EditorStyles.foldout.fontSize,
                FontStyle.Normal);

            EditorVisualControls.Foldout("Layout Helper", ref _showCustomProperties, () =>
            {
                ++EditorGUI.indentLevel;

                DrawVector2FieldLabelAbove("Parent", ref _parentWidth, ref _parentHeight, "Width", "Height");
                DrawVector2FieldLabelAbove("Content", ref _contentWidth, ref _contentHeight, "Width", "Height");

                EditorGUILayout.Space();

                DrawVector2Field("Left/Right Margin", ref _leftMarginWidth, ref _rightMarginWidth, "L", "R");
                DrawVector2Field("Top/Bottom Margin", ref _topMarginHeight, ref _bottomMarginHeight, "T", "B");

                --EditorGUI.indentLevel;

                EditorGUILayout.Space();

                EditorVisualControls.Button("Apply anchors", () => ApplyAnchors(),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }, foldoutStyle);

            EditorPrefs.SetBool("RectTransformExtended.showLayoutHelper", _showCustomProperties);
        }

        private void ApplyAnchors()
        {
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

        static RectTransformExtendedEditor()
        {
            var editorGUIType = typeof(EditorGUI);

            _multiFieldPrefixLabelMethod = editorGUIType.GetMethod("MultiFieldPrefixLabel",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, new[] { typeof(Rect), typeof(int), typeof(GUIContent), typeof(int) }, null);

            _calcPrefixLabelWidthMethod = editorGUIType.GetMethod("CalcPrefixLabelWidth",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, new[] { typeof(GUIContent), typeof(GUIStyle) }, null);
        }

        private void DrawVector2FieldLabelAbove(string mainLabel, ref float xValue, ref float yValue,
            string xLabel = "X", string yLabel = "Y")
        {
            var controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 2f);
            controlRect.height = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(controlRect.x, controlRect.y, EditorGUIUtility.labelWidth, controlRect.height);
            EditorVisualControls.LabelField(labelRect, mainLabel);

            var firstColumnRect = GetColumnRect(controlRect, 0);
            var secondColumnRect = GetColumnRect(controlRect, 1);

            firstColumnRect.height = EditorGUIUtility.singleLineHeight * 2f;
            secondColumnRect.height = EditorGUIUtility.singleLineHeight * 2f;

            var xLabelRect = new Rect(firstColumnRect.x, firstColumnRect.y, firstColumnRect.width,
                EditorGUIUtility.singleLineHeight);
            var xFieldRect = new Rect(firstColumnRect.x, firstColumnRect.y + EditorGUIUtility.singleLineHeight,
                firstColumnRect.width, EditorGUIUtility.singleLineHeight);

            var yLabelRect = new Rect(secondColumnRect.x, secondColumnRect.y, secondColumnRect.width,
                EditorGUIUtility.singleLineHeight);
            var yFieldRect = new Rect(secondColumnRect.x, secondColumnRect.y + EditorGUIUtility.singleLineHeight,
                secondColumnRect.width, EditorGUIUtility.singleLineHeight);

            EditorVisualControls.LabelField(xLabelRect, xLabel);
            EditorVisualControls.LabelField(yLabelRect, yLabel);

            xValue = EditorStateControls.FloatField(xFieldRect, $"{mainLabel} {xLabel}", xValue);
            yValue = EditorStateControls.FloatField(yFieldRect, $"{mainLabel} {yLabel}", yValue);
        }

        private void DrawVector2Field(string label, ref float xValue, ref float yValue, string xLabel = "X",
            string yLabel = "Y")
        {
            var controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            var controlId = GUIUtility.GetControlID(label.GetHashCode(), FocusType.Keyboard, controlRect);

            var fieldRect = (Rect)_multiFieldPrefixLabelMethod.Invoke(null,
                new object[] { controlRect, controlId, new GUIContent(label), 2 });

            var originalLabelWidth = EditorGUIUtility.labelWidth;
            var originalIndentLevel = EditorGUI.indentLevel;

            var subLabelWidth = (float)_calcPrefixLabelWidthMethod.Invoke(null,
                new object[] { new GUIContent(xLabel), EditorStyles.label });

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = subLabelWidth;

            var columnWidth = (fieldRect.width - 2f) / 2f;

            var xRect = new Rect(fieldRect.x, fieldRect.y, columnWidth, fieldRect.height);
            var yRect = new Rect(fieldRect.x + columnWidth + 2f, fieldRect.y, columnWidth, fieldRect.height);

            xValue = EditorStateControls.FloatField(xRect, $"{label} {xLabel}", new GUIContent(xLabel), xValue);
            yValue = EditorStateControls.FloatField(yRect, $"{label} {yLabel}", new GUIContent(yLabel), yValue);

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.indentLevel = originalIndentLevel;
        }

        private Rect GetColumnRect(Rect totalRect, int column)
        {
            totalRect.xMin += EditorGUIUtility.labelWidth - 1f;
            var columnRect = totalRect;
            columnRect.xMin += (float)((totalRect.width - 4.0) * (column / 3.0)) + column * 2;
            columnRect.width = (float)((totalRect.width - 4.0) / 3.0);
            return columnRect;
        }
    }
}
#endif