using System.Reflection;
using CustomUtils.Editor.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Editor.UI
{
    [CustomEditor(typeof(AspectRatioFitter), true)]
    [CanEditMultipleObjects]
    internal sealed class AspectRatioFitterExtendedEditor : EditorBase
    {
        private UnityEditor.Editor _defaultEditor;

        private float _desiredWidth;
        private float _desiredHeight;

        protected override void InitializeEditor()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var aspectRatioFitterEditorType = assembly.GetType("UnityEditor.UI.AspectRatioFitterEditor");

            if (aspectRatioFitterEditorType != null)
                _defaultEditor = CreateEditor(targets, aspectRatioFitterEditorType);
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
            if (_defaultEditor)
                _defaultEditor.OnInspectorGUI();
            else
                DrawDefaultInspector();

            serializedObject.Update();

            DrawCalculatorContent();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCalculatorContent()
        {
            EditorGUILayout.Space();

            DrawSizeFields();

            EditorGUILayout.Space();

            DrawButtonsSection();
        }

        private void DrawSizeFields()
        {
            _desiredWidth = EditorStateControls.FloatField("Desired Width", _desiredWidth);
            _desiredHeight = EditorStateControls.FloatField("Desired Height", _desiredHeight);
        }

        private void DrawButtonsSection()
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorVisualControls.Button("Apply Ratio", ApplyCalculatedRatio);
            EditorVisualControls.Button("Get Current Size", GetCurrentRectSize);
        }

        private void ApplyCalculatedRatio()
        {
            if (_desiredHeight <= 0)
            {
                Debug.LogWarning("[AspectRatioFitterExtendedEditor::ApplyCalculatedRatio] " +
                                 "Desired height must be greater than 0 to calculate aspect ratio.");
                return;
            }

            var calculatedRatio = _desiredWidth / _desiredHeight;

            foreach (var selectedTarget in targets)
            {
                if (!selectedTarget || selectedTarget is not AspectRatioFitter fitter)
                    continue;

                Undo.RecordObject(fitter, "Set Aspect Ratio");
                fitter.aspectRatio = calculatedRatio;
                EditorUtility.SetDirty(fitter);
            }
        }

        private void GetCurrentRectSize()
        {
            if (targets.Length != 1 || !targets[0] || targets[0] is not AspectRatioFitter fitter)
                return;

            var rectTransform = fitter.GetComponent<RectTransform>();
            if (!rectTransform)
                return;

            var rect = rectTransform.rect;
            _desiredWidth = Mathf.Abs(rect.width);
            _desiredHeight = Mathf.Abs(rect.height);
        }
    }
}