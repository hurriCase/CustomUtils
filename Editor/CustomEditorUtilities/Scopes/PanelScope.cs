using System;
using UnityEditor;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for panels with consistent spacing that can be used with 'using' statements.
    /// Provides consistent spacing before and after content using Unity's standard vertical spacing.
    /// </summary>
    public sealed class PanelScope : IDisposable
    {
        internal PanelScope()
        {
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes the panel scope, adding appropriate spacing after the content.
        /// </summary>
        public void Dispose()
        {
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }
    }
}