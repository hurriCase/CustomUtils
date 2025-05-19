using System;
using UnityEditor;

namespace CustomUtils.Editor.EditorTheme.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Disposable scope for boxed sections that can be used with 'using' statements.
    /// </summary>
    public sealed class SectionScope : IDisposable
    {
        private static ThemeEditorSettings Settings => ThemeEditorSettings.GetOrCreateSettings();

        private readonly Action _drawContent;
        private readonly bool _endVertical;

        internal SectionScope(string title, bool withBox = true)
        {
            if (withBox)
            {
                var boxStyle = EditorVisualControls.CreateBoxStyle(
                    Settings.BoxPaddingLeft,
                    Settings.BoxPaddingRight,
                    Settings.BoxPaddingTop,
                    Settings.BoxPaddingBottom);

                EditorGUILayout.BeginVertical(boxStyle);
                _endVertical = true;

                if (string.IsNullOrEmpty(title) is false)
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
            else
            {
                _endVertical = false;
            }
        }

        public void Dispose()
        {
            if (!_endVertical)
                return;

            EditorGUILayout.Space(Settings.BoxContentSpacing);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Settings.BoxSpacingAfter);
        }
    }
}