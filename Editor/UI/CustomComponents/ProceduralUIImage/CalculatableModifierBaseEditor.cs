using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEditor;

namespace CustomUtils.Editor.UI.CustomComponents.ProceduralUIImage
{
    [CustomEditor(typeof(CalculatableModifierBase), true)]
    internal sealed class CalculatableModifierBaseEditor : EditorBase
    {
        private CalculatableModifierBase _adaptiveBorder;

        protected override void InitializeEditor()
        {
            _adaptiveBorder = target as CalculatableModifierBase;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorVisualControls.Button("Calculate Radius", _adaptiveBorder.ApplyRadiiFromDesired);

            serializedObject.ApplyModifiedProperties();
        }
    }
}