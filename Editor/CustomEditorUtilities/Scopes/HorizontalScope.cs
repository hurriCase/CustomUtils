using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// A disposable scope for horizontal layout groups.
    /// </summary>
    public sealed class HorizontalScope : System.IDisposable
    {
        internal HorizontalScope(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }

        internal HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }

}