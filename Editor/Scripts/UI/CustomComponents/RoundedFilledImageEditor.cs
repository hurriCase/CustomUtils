using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Runtime.UI.CustomComponents.FilledImage;
using UnityEditor;

namespace CustomUtils.Editor.Scripts.UI.CustomComponents
{
    [CustomEditor(typeof(RoundedFilledImage))]
    [CanEditMultipleObjects]
    internal sealed class RoundedFilledImageEditor : EditorBase
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorStateControls.PropertyField(serializedObject.FindProperty("m_FillAmount"));

            var (_, roundedCapsProperty) = EditorStateControls
                .PropertyField(nameof(RoundedFilledImage.IsRoundedCaps));

            EditorStateControls.PropertyFieldIf(
                roundedCapsProperty.boolValue is false,
                nameof(RoundedFilledImage.RoundedCapResolution));

            EditorStateControls.PropertyField(nameof(RoundedFilledImage.CustomFillOrigin));
            EditorStateControls.PropertyField(nameof(RoundedFilledImage.ThicknessRatio));
            EditorStateControls.PropertyField(nameof(RoundedFilledImage.SegmentsPerRadian));

            serializedObject.ApplyModifiedProperties();
        }
    }
}