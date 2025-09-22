#if IS_RECTTRANSFORM_EXTENDED_ENABLED
using System.Reflection;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.UI.CustomRectTransform
{
    internal sealed class LayoutGUIDrawer
    {
        private static readonly MethodInfo _multiFieldPrefixLabelMethod;
        private static readonly MethodInfo _calcPrefixLabelWidthMethod;

        static LayoutGUIDrawer()
        {
            var editorGUIType = typeof(EditorGUI);

            _multiFieldPrefixLabelMethod = editorGUIType.GetMethod("MultiFieldPrefixLabel",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, new[] { typeof(Rect), typeof(int), typeof(GUIContent), typeof(int) }, null);

            _calcPrefixLabelWidthMethod = editorGUIType.GetMethod("CalcPrefixLabelWidth",
                BindingFlags.Static | BindingFlags.NonPublic,
                null, new[] { typeof(GUIContent), typeof(GUIStyle) }, null);
        }

        internal void DrawVector2FieldStacked(string mainLabel, ref float xValue, ref float yValue, EditorStateControls stateControls, string xLabel = "X", string yLabel = "Y")
        {
            var controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 2f);
            controlRect.height = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(controlRect.x, controlRect.y, EditorGUIUtility.labelWidth, controlRect.height);
            EditorVisualControls.LabelField(labelRect, mainLabel);

            var (firstColumn, secondColumn) = GetTwoColumnRects(controlRect);

            DrawStackedField(firstColumn, xLabel, ref xValue, stateControls);
            DrawStackedField(secondColumn, yLabel, ref yValue, stateControls);
        }

        internal void DrawVector2FieldHorizontal(string label, ref float xValue, ref float yValue, EditorStateControls stateControls, string xLabel = "X", string yLabel = "Y")
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

            var (xRect, yRect) = GetHorizontalFieldRects(fieldRect);

            xValue = stateControls.FloatField(xRect, $"{label} {xLabel}", new GUIContent(xLabel), xValue);
            yValue = stateControls.FloatField(yRect, $"{label} {yLabel}", new GUIContent(yLabel), yValue);

            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.indentLevel = originalIndentLevel;
        }

        internal void DrawVector2ReadOnly(string mainLabel, float xValue, float yValue, string xLabel = "X", string yLabel = "Y")
        {
            var controlRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * 2f);
            controlRect.height = EditorGUIUtility.singleLineHeight;

            var labelRect = new Rect(controlRect.x, controlRect.y, EditorGUIUtility.labelWidth, controlRect.height);
            EditorVisualControls.LabelField(labelRect, mainLabel);

            var (firstColumn, secondColumn) = GetTwoColumnRects(controlRect);

            DrawStackedReadOnlyField(firstColumn, xLabel, xValue);
            DrawStackedReadOnlyField(secondColumn, yLabel, yValue);
        }

        private void DrawStackedReadOnlyField(Rect columnRect, string labelText, float value)
        {
            var labelRect = new Rect(columnRect.x, columnRect.y, columnRect.width, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(columnRect.x, columnRect.y + EditorGUIUtility.singleLineHeight, columnRect.width,
                EditorGUIUtility.singleLineHeight);

            EditorVisualControls.LabelField(labelRect, labelText);
            EditorVisualControls.ReadOnlyFloatField(fieldRect, value);
        }

        private void DrawStackedField(Rect columnRect, string labelText, ref float value, EditorStateControls stateControls)
        {
            var labelRect = new Rect(columnRect.x, columnRect.y, columnRect.width, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(columnRect.x, columnRect.y + EditorGUIUtility.singleLineHeight, columnRect.width,
                EditorGUIUtility.singleLineHeight);

            EditorVisualControls.LabelField(labelRect, labelText);
            value = stateControls.FloatField(fieldRect, labelText, value);
        }

        private (Rect first, Rect second) GetTwoColumnRects(Rect totalRect)
        {
            totalRect.xMin += EditorGUIUtility.labelWidth - 1f;

            var columnWidth = (totalRect.width - 2f) / 2f;

            var firstColumn = new Rect(totalRect.x, totalRect.y, columnWidth, EditorGUIUtility.singleLineHeight * 2f);
            var secondColumn = new Rect(totalRect.x + columnWidth + 2f, totalRect.y, columnWidth,
                EditorGUIUtility.singleLineHeight * 2f);

            return (firstColumn, secondColumn);
        }

        private (Rect xRect, Rect yRect) GetHorizontalFieldRects(Rect fieldRect)
        {
            var columnWidth = (fieldRect.width - 2f) / 2f;

            var xRect = new Rect(fieldRect.x, fieldRect.y, columnWidth, fieldRect.height);
            var yRect = new Rect(fieldRect.x + columnWidth + 2f, fieldRect.y, columnWidth, fieldRect.height);

            return (xRect, yRect);
        }
    }
}
#endif