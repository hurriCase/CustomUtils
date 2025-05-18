using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
namespace CustomUtils.Editor.EditorTheme
{
    /// <summary>
    /// Static utility class for editor layout operations that don't require undo support.
    /// </summary>
    /// <remarks>
    /// Provides consistent styling and layout methods for creating uniform editor UI elements.
    /// All methods use the centralized theme settings from <see cref="ThemeEditorSettings"/>.
    /// </remarks>
    [UsedImplicitly]
    public static class EditorVisualControls
    {
        private static ThemeEditorSettings Settings => ThemeEditorSettings.GetOrCreateSettings();

        /// <summary>
        /// Draws a section header with consistent styling.
        /// </summary>
        /// <param name="title">The title text to display in the header.</param>
        /// <remarks>
        /// Creates a header with spacing, font size, style, and alignment defined in the theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawSectionHeader(string title)
        {
            var headerStyle = CreateTextStyle(
                EditorStyles.boldLabel,
                Settings.HeaderFontSize,
                Settings.HeaderFontStyle,
                Settings.HeaderAlignment);

            EditorGUILayout.Space(Settings.HeaderSpacing);
            EditorGUILayout.LabelField(title, headerStyle);
        }

        /// <summary>
        /// Creates a panel with consistent spacing around content.
        /// </summary>
        /// <param name="drawContent">Action to execute for drawing the panel content.</param>
        /// <remarks>
        /// Adds consistent spacing before and after the content based on theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawPanel(Action drawContent)
        {
            EditorGUILayout.Space(Settings.PanelSpacing);
            drawContent?.Invoke();
            EditorGUILayout.Space(Settings.PanelSpacing);
        }

        /// <summary>
        /// Creates a property field with consistent styling.
        /// </summary>
        /// <param name="property">The serialized property to display.</param>
        /// <param name="label">The label text to display next to the property field.</param>
        /// <remarks>
        /// Uses consistent height for property fields defined in theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawPropertyFieldWithLabel(SerializedProperty property, string label)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(label), GUILayout.Height(Settings.PropertyHeight));
        }

        /// <summary>
        /// Creates a group of property fields in a horizontal layout.
        /// </summary>
        /// <param name="properties">Array of tuples containing (property, label) pairs to display.</param>
        /// <remarks>
        /// Useful for displaying related properties side-by-side to save vertical space.
        /// </remarks>
        [UsedImplicitly]
        public static void HorizontalProperties(params (SerializedProperty property, string label)[] properties)
        {
            EditorGUILayout.BeginHorizontal();

            foreach (var (property, label) in properties)
                DrawPropertyFieldWithLabel(property, label);

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a warning message box with consistent styling.
        /// </summary>
        /// <param name="message">The warning message to display.</param>
        /// <remarks>
        /// Adds consistent spacing before and after the warning box based on theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void WarningBox(string message)
        {
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
        }

        /// <summary>
        /// Creates an information message box with consistent styling.
        /// </summary>
        /// <param name="message">The information message to display.</param>
        /// <remarks>
        /// Adds consistent spacing before and after the info box based on theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void InfoBox(string message)
        {
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
            EditorGUILayout.HelpBox(message, MessageType.Info);
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
        }

        /// <summary>
        /// Creates a plain help box with consistent styling without an icon.
        /// </summary>
        /// <param name="message">The help message to display.</param>
        /// <remarks>
        /// Adds consistent spacing before and after the help box based on theme settings.
        /// Uses MessageType.None to display a box without an icon.
        /// </remarks>
        [UsedImplicitly]
        public static void HelpBox(string message)
        {
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
            EditorGUILayout.HelpBox(message, MessageType.None);
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
        }

        /// <summary>
        /// Creates an error message box with consistent styling.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <remarks>
        /// Adds consistent spacing before and after the error box based on theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void ErrorBox(string message)
        {
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
            EditorGUILayout.HelpBox(message, MessageType.Error);
            EditorGUILayout.Space(Settings.MessageBoxSpacing);
        }

        /// <summary>
        /// Creates a boxed section with title and content.
        /// </summary>
        /// <param name="title">The title to display at the top of the box. Can be null or empty for no title.</param>
        /// <param name="drawContent">Action to execute for drawing the box content.</param>
        /// <remarks>
        /// Creates a visually distinct boxed area with consistent padding and spacing.
        /// The box styling, title font, and spacing are all defined in theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawBoxedSection(string title, Action drawContent)
        {
            EditorGUILayout.Space(Settings.BoxSpacingBefore);

            var boxStyle = CreateBoxStyle(
                Settings.BoxPaddingLeft,
                Settings.BoxPaddingRight,
                Settings.BoxPaddingTop,
                Settings.BoxPaddingBottom);

            var headerStyle = CreateTextStyle(
                EditorStyles.boldLabel,
                Settings.BoxHeaderFontSize,
                Settings.BoxHeaderFontStyle,
                Settings.BoxHeaderAlignment);

            EditorGUILayout.BeginVertical(boxStyle);

            if (string.IsNullOrEmpty(title) is false)
            {
                EditorGUILayout.Space(Settings.BoxTitleSpacing);
                EditorGUILayout.LabelField(title, headerStyle);
                EditorGUILayout.Space(Settings.BoxTitleSpacing);
            }

            if (drawContent != null)
            {
                EditorGUILayout.Space(Settings.BoxContentSpacing);
                drawContent();
                EditorGUILayout.Space(Settings.BoxContentSpacing);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Settings.BoxSpacingAfter);
        }

        /// <summary>
        /// Creates a boxed section with a foldout header for collapsible content.
        /// </summary>
        /// <param name="title">The title to display in the foldout header.</param>
        /// <param name="foldout">Reference to a boolean that tracks the expanded/collapsed state.</param>
        /// <param name="drawContent">Action to execute for drawing the box content when expanded.</param>
        /// <remarks>
        /// Creates a collapsible section with consistent styling. The foldout state parameter
        /// allows the calling code to persist the expanded/collapsed state between draws.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawBoxWithFoldout(string title, ref bool foldout, Action drawContent)
        {
            EditorGUILayout.Space(Settings.FoldoutBoxSpacingBefore);

            var boxStyle = CreateBoxStyle(
                Settings.FoldoutBoxPaddingLeft,
                Settings.FoldoutBoxPaddingRight,
                Settings.FoldoutBoxPaddingTop,
                Settings.FoldoutBoxPaddingBottom);

            var foldoutStyle = CreateTextStyle(
                EditorStyles.foldout,
                Settings.FoldoutFontSize,
                Settings.FoldoutFontStyle);

            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.Space(Settings.FoldoutHeaderSpacing);

            foldout = EditorGUILayout.Foldout(foldout, title, true, foldoutStyle);

            if (foldout && drawContent != null)
            {
                EditorGUILayout.Space(Settings.FoldoutContentSpacing);
                EditorGUILayout.BeginVertical();
                drawContent();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(Settings.FoldoutContentSpacing);
            }

            EditorGUILayout.Space(Settings.FoldoutHeaderSpacing);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Settings.FoldoutBoxSpacingAfter);
        }

        [UsedImplicitly]
        public static bool Button(string text, params GUILayoutOption[] options)
        {
            var buttonStyle = CreateTextStyle(GUI.skin.button, Settings.ButtonFontSize, Settings.ButtonFontStyle);
            return GUILayout.Button(text, buttonStyle, options);
        }

        [UsedImplicitly]
        public static bool Foldout(bool foldout, string title, bool toggleOnLabelClick = true)
        {
            var foldoutStyle = CreateTextStyle(
                EditorStyles.foldout,
                Settings.FoldoutFontSize,
                Settings.FoldoutFontStyle);

            return EditorGUILayout.Foldout(foldout, title, toggleOnLabelClick, foldoutStyle);
        }

        /// <summary>
        /// Creates a primary heading (H1) with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display as a heading.</param>
        /// <remarks>
        /// This method applies the largest heading style based on theme settings.
        /// Use for main section titles or page headers.
        /// </remarks>
        [UsedImplicitly]
        public static void H1Label(string text)
        {
            var headerStyle = CreateTextStyle(
                EditorStyles.boldLabel,
                Settings.H1FontSize,
                FontStyle.Bold,
                Settings.HeaderAlignment);

            var heightStyle = GUILayout.Height(Settings.H1FontSize * Settings.FontHeightScaleFactor);

            EditorGUILayout.Space(Settings.H1SpacingBefore);
            EditorGUILayout.LabelField(text, headerStyle, heightStyle);
            EditorGUILayout.Space(Settings.H1SpacingAfter);
        }

        /// <summary>
        /// Creates a secondary heading (H2) with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display as a heading.</param>
        /// <remarks>
        /// This method applies medium-sized heading styling based on theme settings.
        /// Use for section headers within a larger content area.
        /// </remarks>
        [UsedImplicitly]
        public static void H2Label(string text)
        {
            var headerStyle = CreateTextStyle(
                EditorStyles.boldLabel,
                Settings.H2FontSize,
                FontStyle.Bold,
                Settings.HeaderAlignment);

            var heightStyle = GUILayout.Height(Settings.H2FontSize * Settings.FontHeightScaleFactor);

            EditorGUILayout.Space(Settings.H2SpacingBefore);
            EditorGUILayout.LabelField(text, headerStyle, heightStyle);
            EditorGUILayout.Space(Settings.H2SpacingAfter);
        }

        /// <summary>
        /// Creates a tertiary heading (H3) with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display as a heading.</param>
        /// <remarks>
        /// This method applies smaller heading styling based on theme settings.
        /// Use for subsection headers or group titles.
        /// </remarks>
        [UsedImplicitly]
        public static void H3Label(string text)
        {
            var headerStyle = CreateTextStyle(
                EditorStyles.boldLabel,
                Settings.H3FontSize,
                FontStyle.Bold,
                Settings.HeaderAlignment);

            var heightStyle = GUILayout.Height(Settings.H3FontSize * Settings.FontHeightScaleFactor);

            EditorGUILayout.Space(Settings.H3SpacingBefore);
            EditorGUILayout.LabelField(text, headerStyle, heightStyle);
            EditorGUILayout.Space(Settings.H3SpacingAfter);
        }

        /// <summary>
        /// Creates a standard label with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display in the label.</param>
        /// <remarks>
        /// This method applies standard text styling based on theme settings.
        /// Use for regular text content that needs consistent styling.
        /// </remarks>
        [UsedImplicitly]
        public static void LabelField(string text)
        {
            var labelStyle = CreateTextStyle(
                EditorStyles.label,
                Settings.LabelFontSize,
                Settings.LabelFontStyle);

            EditorGUILayout.LabelField(text, labelStyle);
        }

        /// <summary>
        /// Creates a standard label with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display in the label.</param>
        /// <param name="style">Additional style to apply to the label.</param>
        /// <remarks>
        /// This method applies both the theme's standard text styling and any custom styles provided.
        /// Use when you need a consistently styled label with additional custom styling.
        /// </remarks>
        [UsedImplicitly]
        public static void LabelField(string text, GUIStyle style)
        {
            var combinedStyle = new GUIStyle(style)
            {
                fontSize = Settings.LabelFontSize,
                fontStyle = Settings.LabelFontStyle
            };

            EditorGUILayout.LabelField(text, combinedStyle);
        }

        /// <summary>
        /// Creates a selectable label that wraps text and automatically adjusts its height based on content.
        /// </summary>
        /// <param name="text">The text to display in the selectable label.</param>
        /// <param name="style">The base style to use for the label. If null, EditorStyles.label will be used.</param>
        public static void DrawWrappedSelectableLabel(string text, GUIStyle style = null)
        {
            style = style ?? EditorStyles.label;

            var wrappedStyle = new GUIStyle(style)
            {
                wordWrap = true
            };

            var height = wrappedStyle.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth);

            EditorGUILayout.SelectableLabel(text, wrappedStyle, GUILayout.Height(height));
        }

        /// <summary>
        /// Creates a horizontal line (divider) with consistent styling.
        /// </summary>
        /// <remarks>
        /// Adds a visual separator between UI elements with spacing above and below.
        /// The divider height, color, and spacing are defined in theme settings.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawHorizontalLine()
        {
            EditorGUILayout.Space(Settings.DividerSpacing);

            var rect = EditorGUILayout.GetControlRect(false, Settings.DividerHeight);
            rect.height = Settings.DividerHeight;
            EditorGUI.DrawRect(rect, Settings.DividerColor);

            EditorGUILayout.Space(Settings.DividerSpacing);
        }

        /// <summary>
        /// Creates a text style with consistent styling based on settings.
        /// </summary>
        /// <param name="baseStyle">The base GUIStyle to clone and modify.</param>
        /// <param name="fontSize">Optional custom font size. If null, uses the base style's size.</param>
        /// <param name="fontStyle">Optional custom font style (bold, italic, etc). If null, uses the base style.</param>
        /// <param name="alignment">Optional custom text alignment. If null, uses the base style's alignment.</param>
        /// <returns>A new GUIStyle instance with the specified modifications.</returns>
        /// <remarks>
        /// This utility method is used internally to create consistent text styles throughout the editor.
        /// </remarks>
        [UsedImplicitly]
        public static GUIStyle CreateTextStyle(GUIStyle baseStyle, int? fontSize = null, FontStyle? fontStyle = null,
            TextAnchor? alignment = null)
        {
            var style = new GUIStyle(baseStyle)
            {
                fontSize = fontSize ?? baseStyle.fontSize,
                fontStyle = fontStyle ?? baseStyle.fontStyle
            };

            if (alignment.HasValue)
                style.alignment = alignment.Value;

            return style;
        }

        /// <summary>
        /// Creates a box style with consistent styling based on settings.
        /// </summary>
        /// <param name="paddingLeft">Left padding in pixels.</param>
        /// <param name="paddingRight">Right padding in pixels.</param>
        /// <param name="paddingTop">Top padding in pixels.</param>
        /// <param name="paddingBottom">Bottom padding in pixels.</param>
        /// <returns>A new GUIStyle instance configured for box display.</returns>
        /// <remarks>
        /// This utility method is used internally to create consistent box styles throughout the editor.
        /// </remarks>
        [UsedImplicitly]
        public static GUIStyle CreateBoxStyle(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom)
        {
            var style = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(
                    paddingLeft,
                    paddingRight,
                    paddingTop,
                    paddingBottom)
            };

            return style;
        }

        /// <summary>
        /// Draws a progress bar with title and progress value
        /// </summary>
        /// <param name="title">Title of the progress operation</param>
        /// <param name="info">Additional information to display</param>
        /// <param name="progress">Progress value between 0 and 1</param>
        [UsedImplicitly]
        public static void DrawProgressBar(string title, string info, float progress)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (string.IsNullOrEmpty(info) is false)
                EditorGUILayout.LabelField(info, EditorStyles.miniLabel);

            var progressRect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(progressRect, Mathf.Clamp01(progress), $"{Mathf.RoundToInt(progress * 100)}%");

            EditorGUILayout.EndVertical();
        }
    }
}