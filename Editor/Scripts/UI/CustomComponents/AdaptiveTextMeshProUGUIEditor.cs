using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Runtime.UI.CustomComponents;
using TMPro.EditorUtilities;
using UnityEditor;

namespace CustomUtils.Editor.Scripts.UI.CustomComponents
{
    [CustomEditor(typeof(AdaptiveTextMeshProUGUI), true)]
    [CanEditMultipleObjects]
    internal sealed class AdaptiveTextMeshProUGUIEditor : TMP_EditorPanelUI
    {
        private EditorStateControls _editorStateControls;

        protected override void OnEnable()
        {
            base.OnEnable();

            _editorStateControls = new EditorStateControls(target, serializedObject);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorVisualControls.LabelField("Adaptive Settings");

            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.StaticDimensionType));
            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.BaseFontSize));
            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.StaticReferenceSize));
            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.ExpandToFitText));

            serializedObject.ApplyModifiedProperties();
        }
    }
}