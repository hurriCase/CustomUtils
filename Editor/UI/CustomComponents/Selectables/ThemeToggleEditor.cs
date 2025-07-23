using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI.CustomComponents.Selectables;
using UnityEditor;
using UnityEditor.UI;

namespace CustomUtils.Editor.UI.CustomComponents.Selectables
{
    [CustomEditor(typeof(ThemeToggle))]
    internal sealed class ThemeToggleEditor : ToggleEditor
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

            _editorStateControls.PropertyField(nameof(ThemeToggle.SelectableColorMapping));
            _editorStateControls.PropertyField(nameof(ThemeToggle.AdditionalGraphics), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}