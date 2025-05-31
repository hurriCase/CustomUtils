using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace CustomUtils.Editor.UI
{
    [CustomEditor(typeof(RoundedFilledImageComponent), true)]
    [CanEditMultipleObjects]
    internal sealed class RoundedFilledImageComponentEditor : ImageEditor
    {
        private SerializedProperty _typeProperty;
        private SerializedProperty _fillMethodProperty;

        private EditorStateControls _editorStateControls;

        protected override void OnEnable()
        {
            base.OnEnable();

            _typeProperty = serializedObject.FindProperty("m_Type");
            _fillMethodProperty = serializedObject.FindProperty("m_FillMethod");

            _editorStateControls ??= new EditorStateControls(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorVisualControls.LabelField("Rounded Fill Settings", EditorStyles.boldLabel);

            DrawRoundedSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRoundedSettings()
        {
            var isRadial360 = _typeProperty.enumValueIndex == (int)Image.Type.Filled &&
                              _fillMethodProperty.enumValueIndex == (int)Image.FillMethod.Radial360;

            using var disabledScope = new EditorGUI.DisabledScope(isRadial360 is false);

            var (_, roundedCapsProperty) = _editorStateControls
                .PropertyField(nameof(RoundedFilledImageComponent.RoundedCaps));

            _editorStateControls.PropertyFieldIf(
                roundedCapsProperty.boolValue is false,
                nameof(RoundedFilledImageComponent.RoundedCapResolution));

            var (_, useCustomFillOriginProperty) = _editorStateControls
                .PropertyField(nameof(RoundedFilledImageComponent.UseCustomFillOrigin));

            _editorStateControls.PropertyFieldIf(
                useCustomFillOriginProperty.boolValue is false,
                nameof(RoundedFilledImageComponent.CustomFillOrigin));

            _editorStateControls.PropertyField(nameof(RoundedFilledImageComponent.ThicknessRatio));

            if (isRadial360 is false && roundedCapsProperty.boolValue)
                EditorVisualControls.WarningBox("Rounded caps only work with Radial 360 fill method.");
        }
    }
}