using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.EditorTheme.Scopes
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

        public HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }

}