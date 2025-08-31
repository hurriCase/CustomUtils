using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI.CustomComponents.Selectables;
using UnityEditor;
using UnityEditor.UI;

namespace CustomUtils.Editor.UI.CustomComponents.Selectables
{
    [CustomEditor(typeof(ThemeButton), true)]
    internal sealed class ThemeButtonEditor : ButtonEditor
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

            EditorVisualControls.LabelField("Custom Settings");

            _editorStateControls.PropertyField(nameof(ThemeButton.Text), true);
            _editorStateControls.PropertyField(nameof(ThemeButton.GraphicMappings), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}