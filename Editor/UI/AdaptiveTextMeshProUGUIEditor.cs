using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI;
using TMPro.EditorUtilities;
using UnityEditor;

namespace CustomUtils.Editor.UI
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

            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.DimensionType));
            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.BaseFontSize));
            _editorStateControls.PropertyField(nameof(AdaptiveTextMeshProUGUI.ReferenceSize));

            serializedObject.ApplyModifiedProperties();
        }
    }
}