using System;
using CustomUtils.Editor.EditorTheme;
using UnityEditor;

namespace CustomUtils.Editor
{
    internal sealed class InputDialogWindow : WindowBase
    {
        internal string Message { get; set; }
        internal string InputText { get; set; }

        internal event Action<string> OnComplete;

        protected override void DrawWindowContent()
        {
            EditorVisualControls.LabelField(Message);

            InputText = EditorGUILayout.PasswordField(string.Empty, InputText);

            EditorGUILayout.BeginHorizontal();
            if (EditorVisualControls.Button("OK"))
            {
                OnComplete?.Invoke(InputText);
                Close();
            }

            if (EditorVisualControls.Button("Cancel"))
            {
                OnComplete?.Invoke("");
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}