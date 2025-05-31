using System;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for scroll views that can be used with 'using' statements.
    /// </summary>
    public sealed class ScrollScope : IDisposable
    {
        internal ScrollScope(ref Vector2 scrollPosition)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}