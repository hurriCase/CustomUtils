using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for horizontal layout groups that can be used with 'using' statements.
    /// Provides automatic cleanup of horizontal layout groups in Unity Editor GUI.
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

        /// <inheritdoc />
        /// <summary>
        /// Disposes the horizontal scope, properly closing the horizontal layout group.
        /// </summary>
        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}