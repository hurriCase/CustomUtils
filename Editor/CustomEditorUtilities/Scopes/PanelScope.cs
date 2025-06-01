using System;
using CustomUtils.Editor.EditorTheme;
using UnityEditor;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for panels with consistent spacing that can be used with 'using' statements.
    /// Provides consistent spacing before and after content based on theme settings.
    /// </summary>
    public sealed class PanelScope : IDisposable
    {
        private static ThemeEditorSettings Settings => ThemeEditorSettings.Instance;

        /// <summary>
        /// Creates a new panel scope with consistent spacing.
        /// </summary>
        internal PanelScope()
        {
            EditorGUILayout.Space(Settings.PanelSpacing);
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes the panel scope, adding appropriate spacing after the content.
        /// </summary>
        public void Dispose()
        {
            EditorGUILayout.Space(Settings.PanelSpacing);
        }
    }
}