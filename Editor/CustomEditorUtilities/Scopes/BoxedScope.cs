using System;
using CustomUtils.Editor.EditorTheme;
using UnityEditor;

namespace CustomUtils.Editor.CustomEditorUtilities.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for boxed sections with optional titles that can be used with 'using' statements.
    /// Provides consistent styling and spacing for creating visually distinct sections.
    /// </summary>
    public sealed class BoxedScope : IDisposable
    {
        private static ThemeEditorSettings Settings => ThemeEditorSettings.Instance;

        /// <summary>
        /// Creates a new boxed scope with optional title.
        /// </summary>
        /// <param name="title">Optional title to display at the top of the box. Can be null or empty for no title.</param>
        internal BoxedScope(string title = null)
        {
            var hasTitle = string.IsNullOrEmpty(title) is false;

            EditorGUILayout.Space(Settings.BoxSpacingBefore);

            var boxStyle = EditorVisualControls.CreateBoxStyle(
                Settings.BoxPaddingLeft,
                Settings.BoxPaddingRight,
                Settings.BoxPaddingTop,
                Settings.BoxPaddingBottom);

            EditorGUILayout.BeginVertical(boxStyle);

            if (hasTitle)
            {
                var headerStyle = EditorVisualControls.CreateTextStyle(
                    EditorStyles.boldLabel,
                    Settings.BoxHeaderFontSize,
                    Settings.BoxHeaderFontStyle,
                    Settings.BoxHeaderAlignment);

                EditorGUILayout.Space(Settings.BoxTitleSpacing);
                EditorGUILayout.LabelField(title, headerStyle);
                EditorGUILayout.Space(Settings.BoxTitleSpacing);
            }

            EditorGUILayout.Space(Settings.BoxContentSpacing);
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes the boxed scope, ending the vertical layout and adding appropriate spacing.
        /// </summary>
        public void Dispose()
        {
            EditorGUILayout.Space(Settings.BoxContentSpacing);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(Settings.BoxSpacingAfter);
        }
    }
}