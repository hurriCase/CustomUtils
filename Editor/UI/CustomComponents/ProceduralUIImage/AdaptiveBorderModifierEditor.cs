using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI.CustomComponents.ProceduralUIImage
{
    [CustomEditor(typeof(AdaptiveBorderModifier))]
    internal sealed class AdaptiveBorderModifierEditor : EditorBase
    {
        private SerializedProperty _cornerRadiusRatio;
        private SerializedProperty _desiredRadius;
        private RectTransform _rectTransform;

        protected override void InitializeEditor()
        {
            _desiredRadius = serializedObject.FindField(nameof(AdaptiveBorderModifier.DesiredRadius));
            _cornerRadiusRatio = serializedObject.FindField(nameof(AdaptiveBorderModifier.CornerRadiusRatio));

            var adaptiveBorder = target as AdaptiveBorderModifier;
            _rectTransform = adaptiveBorder ? adaptiveBorder.GetComponent<RectTransform>() : null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorStateControls.PropertyField(_desiredRadius);
            EditorStateControls.PropertyField(_cornerRadiusRatio);

            EditorVisualControls.Button("Calculate Radius", CalculateRadius);

            serializedObject.ApplyModifiedProperties();
        }

        private void CalculateRadius()
        {
            if (!_rectTransform)
                return;

            var rect = _rectTransform.rect;
            var minSize = Mathf.Min(rect.width, rect.height);
            var radius = _desiredRadius.floatValue / minSize;
            _cornerRadiusRatio.floatValue = radius;
        }
    }
}