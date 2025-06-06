using System;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for scroll views that can be used with 'using' statements.
    /// Provides automatic cleanup of scroll view layouts in Unity Editor GUI.
    /// </summary>
    public sealed class ScrollScope : IDisposable
    {
        internal ScrollScope(ref Vector2 scrollPosition)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes the scroll scope, properly closing the scroll view layout.
        /// </summary>
        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}