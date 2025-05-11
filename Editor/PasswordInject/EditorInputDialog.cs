using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.PasswordInject
{
    internal static class EditorInputDialog
    {
        internal static string Show(string title, string message, string inputText)
        {
            var result = inputText;

            var window = EditorWindow.GetWindow<InputDialogWindow>(true, title, true);
            window.titleContent = new GUIContent(title);
            window.Message = message;
            window.InputText = inputText;
            window.OnComplete += text => { result = text; };
            window.ShowModalUtility();

            return result;
        }
    }
}