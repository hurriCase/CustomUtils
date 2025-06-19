using CustomUtils.Editor.CustomEditorUtilities;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI.AspectRatio
{
    internal abstract class AspectRatioEditorBase<T> : EditorBase where T : Component
    {
        private float _desiredWidth;
        private float _desiredHeight;

        public override void OnInspectorGUI()
        {
            DrawMainInspector();

            serializedObject.Update();

            DrawCalculatorSection();

            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void DrawMainInspector();

        protected abstract RectTransform GetRectTransform(T component);

        protected abstract void SetAspectRatio(T component, float aspectRatio);

        protected virtual string UndoOperationName => "Set Aspect Ratio";

        private void DrawCalculatorSection()
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
            var calculatedRatio = CalculateAspectRatio(_desiredWidth, _desiredHeight);

            if (calculatedRatio <= 0)
                return;

            foreach (var selectedTarget in targets)
            {
                if (!selectedTarget || selectedTarget is not T component)
                    continue;

                Undo.RecordObject(component, UndoOperationName);
                SetAspectRatio(component, calculatedRatio);
                EditorUtility.SetDirty(component);
            }
        }

        private void GetCurrentRectSize()
        {
            if (targets.Length != 1 || !targets[0] || targets[0] is not T component)
                return;

            var rectTransform = GetRectTransform(component);
            if (!rectTransform)
                return;

            var size = GetRectTransformSize(rectTransform);
            _desiredWidth = size.x;
            _desiredHeight = size.y;
        }

        private static float CalculateAspectRatio(float width, float height)
        {
            if ((height <= 0) is false)
                return width / height;

            Debug.LogWarning("[AspectRatioEditorBase::CalculateAspectRatio] " +
                             "Desired height must be greater than 0 to calculate aspect ratio.");
            return 1f;
        }

        private static Vector2 GetRectTransformSize(RectTransform rectTransform)
        {
            if (!rectTransform)
                return Vector2.zero;

            var rect = rectTransform.rect;
            return new Vector2(Mathf.Abs(rect.width), Mathf.Abs(rect.height));
        }
    }
}