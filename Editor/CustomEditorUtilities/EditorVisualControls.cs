using System;
using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities.Scopes;
using CustomUtils.Editor.EditorTheme;
using CustomUtils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
namespace CustomUtils.Editor.CustomEditorUtilities
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
        private static ThemeEditorSettings Settings => ThemeEditorSettings.Instance;

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

            EditorGUILayout.Space(Settings.BoxTitleSpacing);
            EditorGUILayout.LabelField(title, headerStyle);
            EditorGUILayout.Space(Settings.BoxTitleSpacing);

            EditorGUILayout.Space(Settings.BoxContentSpacing);
            drawContent();
            EditorGUILayout.Space(Settings.BoxContentSpacing);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Settings.BoxSpacingAfter);
        }

        /// <summary>
        /// Creates a section scope that can be used with the 'using' statement.
        /// </summary>
        /// <param name="title">Title of the section to display in the header.</param>
        /// <returns>A disposable section scope.</returns>
        /// <remarks>
        /// Use this with the 'using' statement to create a section with clearly defined boundaries.
        /// </remarks>
        [UsedImplicitly]
        public static SectionScope BeginSection(string title)
        {
            EditorGUILayout.Space(Settings.BoxSpacingBefore);
            return new SectionScope(title);
        }

        /// <summary>
        /// Creates a boxed section with a foldout header for collapsible content.
        /// </summary>
        /// <param name="title">The title to display in the foldout header.</param>
        /// <param name="showFoldout">Reference to a boolean that tracks the expanded/collapsed state.</param>
        /// <param name="drawContent">Action to execute for drawing the box content when expanded.</param>
        /// <param name="withFoldout"></param>
        /// <remarks>
        /// Creates a collapsible section with consistent styling. The foldout state parameter
        /// allows the calling code to persist the expanded/collapsed state between draws.
        /// </remarks>
        [UsedImplicitly]
        public static void DrawBoxWithFoldout(string title, ref bool showFoldout, Action drawContent, bool withFoldout = true)
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

            FoldoutWithContent(title, ref showFoldout, drawContent, foldoutStyle);

            EditorGUILayout.Space(Settings.FoldoutHeaderSpacing);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(Settings.FoldoutBoxSpacingAfter);
        }

        /// <summary>
        /// Creates a simple foldout with wrapped content display without styling.
        /// </summary>
        /// <param name="title">The title to display next to the foldout arrow.</param>
        /// <param name="showFoldout">Current expanded state of the foldout.</param>
        /// <param name="drawContent">Action to execute for drawing the content when the foldout is expanded.</param>
        /// <param name="foldoutStyle">The GUIStyle to use for rendering the foldout control.</param>
        /// <returns>The new expanded state of the foldout.</returns>
        /// <remarks>
        /// This method provides a simple foldout mechanism that ensures content is properly wrapped
        /// without any truncation. No custom styling is applied.
        /// </remarks>
        [UsedImplicitly]
        public static bool FoldoutWithContent(string title, ref bool showFoldout, Action drawContent, GUIStyle foldoutStyle = null)
        {
            showFoldout = EditorGUILayout.Foldout(showFoldout, title, true, foldoutStyle);

            if (showFoldout is false || drawContent == null)
                return showFoldout;

            EditorGUILayout.BeginVertical();
            drawContent();
            EditorGUILayout.EndVertical();

            return true;
        }

        /// <summary>
        /// Creates a foldout control with consistent text styling from theme settings.
        /// </summary>
        /// <param name="title">The title to display next to the foldout arrow.</param>
        /// <param name="showFoldout">Current expanded state of the foldout.</param>
        /// /// <param name="drawContent">Action to execute for drawing the content when the foldout is expanded.</param>
        /// <param name="toggleOnLabelClick">Whether clicking the label toggles the foldout state (true) or only the arrow does (false).</param>
        /// <returns>The new expanded state of the foldout.</returns>
        /// <remarks>
        /// This method applies consistent font size and style from theme settings to maintain UI consistency.
        /// </remarks>
        [UsedImplicitly]
        public static bool Foldout(string title, ref bool showFoldout, Action drawContent, bool toggleOnLabelClick = true)
        {
            var foldoutStyle = CreateTextStyle(
                EditorStyles.foldout,
                Settings.FoldoutFontSize,
                Settings.FoldoutFontStyle);

            showFoldout = EditorGUILayout.Foldout(showFoldout, title, toggleOnLabelClick, foldoutStyle);
            if (showFoldout)
                drawContent.Invoke();

            return showFoldout;
        }

        /// <summary>
        /// Creates a foldout section with styled header and expandable content.
        /// </summary>
        /// <param name="title">The title text to display in the foldout header.</param>
        /// <param name="showFoldouts">List of boolean values tracking the expanded state of multiple foldouts.</param>
        /// <param name="index">The index in the showFoldouts list for this specific foldout.</param>
        /// <param name="drawContent">Action to execute when the foldout is expanded.</param>
        /// <param name="toggleOnLabelClick">Whether clicking on the label toggles the foldout state.</param>
        /// <returns>The updated list of foldout states.</returns>
        /// <remarks>
        /// Uses theme settings for consistent font size and style across the editor interface.
        /// </remarks>
        [UsedImplicitly]
        public static List<bool> Foldout(string title, in List<bool> showFoldouts, int index, Action drawContent, bool toggleOnLabelClick = true)
        {
            var foldoutStyle = CreateTextStyle(
                EditorStyles.foldout,
                Settings.FoldoutFontSize,
                Settings.FoldoutFontStyle);

            var showDetail = showFoldouts.GetOrCreate(index);
            showDetail = EditorGUILayout.Foldout(showDetail, title, toggleOnLabelClick, foldoutStyle);
            if (showDetail)
                drawContent.Invoke();

            showFoldouts[index] = showDetail;

            return showFoldouts;
        }

        /// <summary>
        /// Creates a standard button with consistent styling.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="options">Optional GUILayout options for customizing button appearance and behavior.</param>
        /// <returns>True if the button was clicked in this frame, false otherwise.</returns>
        [UsedImplicitly]
        public static bool Button(string text, params GUILayoutOption[] options) => GUILayout.Button(text, options);

        /// <summary>
        /// Creates a button that executes an action when clicked.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="drawContent">Action to execute when the button is clicked. Can be null.</param>
        /// <param name="options">Optional GUILayout options for customizing button appearance and behavior.</param>
        [UsedImplicitly]
        public static void Button(string text, Action drawContent, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, options))
                drawContent?.Invoke();
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
        /// <param name="image">The texture to display on the label.</param>
        /// <remarks>
        /// This method applies standard text styling based on theme settings.
        /// Use for regular text content that needs consistent styling.
        /// </remarks>
        [UsedImplicitly]
        public static void Label(Texture image)
        {
            var labelStyle = CreateTextStyle(
                EditorStyles.label,
                Settings.LabelFontSize,
                Settings.LabelFontStyle);

            GUILayout.Label(image, labelStyle);
        }

        /// <summary>
        /// Creates a standard label with consistent styling from theme settings.
        /// </summary>
        /// <param name="image">The texture to display on the label.</param>
        /// <param name="options">Additional layout options to apply to the label.</param>
        /// <remarks>
        /// This method applies both the theme's standard text styling and any custom styles provided.
        /// Use when you need a consistently styled label with additional custom styling.
        /// </remarks>
        [UsedImplicitly]
        public static void Label(Texture image, params GUILayoutOption[] options)
        {
            var combinedStyle = new GUIStyle()
            {
                fontSize = Settings.LabelFontSize,
                fontStyle = Settings.LabelFontStyle
            };

            GUILayout.Label(image, combinedStyle, options);
        }

        /// <summary>
        /// Creates a standard label with consistent styling from theme settings.
        /// </summary>
        /// <param name="image">The texture to display on the label.</param>
        /// <param name="style">Additional style to apply to the label</param>
        /// <param name="options">Additional layout options to apply to the label.</param>
        /// <remarks>
        /// This method applies both the theme's standard text styling and any custom styles provided.
        /// Use when you need a consistently styled label with additional custom styling.
        /// </remarks>
        [UsedImplicitly]
        public static void Label(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            var combinedStyle = new GUIStyle(style)
            {
                fontSize = Settings.LabelFontSize,
                fontStyle = Settings.LabelFontStyle
            };

            GUILayout.Label(image, combinedStyle, options);
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
        /// Creates a standard label with consistent styling from theme settings.
        /// </summary>
        /// <param name="text">The text to display in the label.</param>
        /// <param name="style">Additional style to apply to the label.</param>
        /// <remarks>
        /// This method applies both the theme's standard text styling and any custom styles provided.
        /// Use when you need a consistently styled label with additional custom styling.
        /// </remarks>
        [UsedImplicitly]
        public static void LabelField(string text, GUILayoutOption style)
        {
            var labelStyle = CreateTextStyle(
                EditorStyles.label,
                Settings.LabelFontSize,
                Settings.LabelFontStyle);

            EditorGUILayout.LabelField(text, labelStyle, style);
        }

        /// <summary>
        /// Creates a selectable label that wraps text and automatically adjusts its height based on content.
        /// </summary>
        /// <param name="text">The text to display in the selectable label.</param>
        /// <param name="style">The base style to use for the label. If null, EditorStyles.label will be used.</param>
        [UsedImplicitly]
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
        /// Creates a scrollable area for the content defined in the action.
        /// Uses the 'using' pattern for automatic cleanup.
        /// </summary>
        /// <param name="scrollPosition">Reference to the scroll position Vector2.</param>
        /// <returns>A disposable scroll scope that handles Begin/End scroll view pairs.</returns>
        [UsedImplicitly]
        public static ScrollScope CreateScrollView(ref Vector2 scrollPosition) => new(ref scrollPosition);

        /// <summary>
        /// Creates a horizontal group and executes the provided action within it.
        /// </summary>
        /// <param name="drawContent">Action to execute inside the horizontal group.</param>
        /// <param name="options">Optional GUILayout options.</param>
        [UsedImplicitly]
        public static void DrawHorizontalGroup(Action drawContent, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            drawContent?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a horizontal group with a specified style and executes the provided action within it.
        /// </summary>
        /// <param name="style">The GUIStyle to use for the horizontal group.</param>
        /// <param name="drawContent">Action to execute inside the horizontal group.</param>
        /// <param name="options">Optional GUILayout options.</param>
        [UsedImplicitly]
        public static void DrawHorizontalGroup(GUIStyle style, Action drawContent, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
            drawContent?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a horizontal group within a box and executes the provided action within it.
        /// </summary>
        /// <param name="drawContent">Action to execute inside the horizontal group.</param>
        /// <param name="options">Optional GUILayout options.</param>
        [UsedImplicitly]
        public static void DrawHorizontalBoxGroup(Action drawContent, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, options);
            drawContent?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a disposable horizontal group scope for use with 'using' statements.
        /// </summary>
        /// <param name="options">Optional GUILayout options.</param>
        /// <returns>A disposable scope object.</returns>
        [UsedImplicitly]
        public static HorizontalScope CreateHorizontalGroup(params GUILayoutOption[] options) => new(options);

        /// <summary>
        /// Creates a disposable horizontal group scope for use with 'using' statements.
        /// </summary>
        /// <param name="style">The GUIStyle to use for the horizontal group.</param>
        /// <param name="options">Optional GUILayout options.</param>
        /// <returns>A disposable scope object.</returns>
        [UsedImplicitly]
        public static HorizontalScope CreateHorizontalGroup(GUIStyle style, params GUILayoutOption[] options)
            => new(style, options);

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
        /// Creates a drag area that accepts Unity Objects.
        /// </summary>
        /// <param name="height">Height of the drop area in pixels.</param>
        /// <param name="message">Message to display in the drop area.</param>
        /// <param name="droppedObject"></param>
        /// <returns>True if objects were dropped in this frame, false otherwise.</returns>
        /// <remarks>
        /// Creates a visual drop area where users can drag Unity Objects.
        /// When objects are dropped, the provided callback is invoked with the list of objects.
        /// </remarks>
        [UsedImplicitly]
        public static bool DrawObjectDropArea(float height, string message, in List<Object> droppedObject)
        {
            var dropArea = GUILayoutUtility.GetRect(0.0f, height, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, message);

            var currentEditorEvent = Event.current;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (currentEditorEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropArea.Contains(currentEditorEvent.mousePosition) is false)
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (currentEditorEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            foreach (var draggedObject in DragAndDrop.objectReferences)
                            {
                                if (draggedObject && droppedObject.AsValueEnumerable().Contains(draggedObject) is false)
                                    droppedObject.Add(draggedObject);
                            }

                            return true;
                        }
                    }

                    currentEditorEvent.Use();
                    break;
            }

            return false;
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

        /// <summary>
        /// Creates a clickable text label with hover cursor indication that executes an action when clicked.
        /// Text automatically wraps to multiple lines if it exceeds the available width.
        /// </summary>
        /// <param name="text">The text to display as a clickable label.</param>
        /// <param name="onClick">Action to execute when the text is clicked. Can be null.</param>
        /// <returns>True if the text was clicked, false otherwise.</returns>
        /// <remarks>
        /// The text appears as a standard label with hand cursor on hover and executes the provided action when clicked.
        /// Uses consistent text styling from theme settings with enhanced user interaction feedback.
        /// Text automatically wraps to new lines when it exceeds the available width, and the control height adjusts accordingly.
        /// This is a wrapper around the URL-opening version that provides more flexibility for custom actions.
        /// </remarks>
        [UsedImplicitly]
        public static bool ClickableTextWithCursor(string text, Action onClick)
        {
            var labelStyle = CreateTextStyle(
                EditorStyles.label,
                Settings.LabelFontSize,
                Settings.LabelFontStyle);

            labelStyle.wordWrap = true;

            var content = new GUIContent(text);
            var availableWidth = EditorGUIUtility.currentViewWidth - GUI.skin.box.padding.horizontal;
            var height = labelStyle.CalcHeight(content, availableWidth);

            var rect = GUILayoutUtility.GetRect(content, labelStyle, GUILayout.Height(height));

            if (GUI.Button(rect, text, labelStyle))
            {
                onClick?.Invoke();
                return true;
            }

            if (rect.Contains(Event.current.mousePosition))
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            return false;
        }

        /// <summary>
        /// Creates a clickable text label with hover cursor indication that opens a URL when clicked.
        /// Text automatically wraps to multiple lines if it exceeds the available width.
        /// </summary>
        /// <param name="text">The text to display as a clickable label.</param>
        /// <param name="url">The URL to open when the text is clicked.</param>
        /// <returns>True if the text was clicked, false otherwise.</returns>
        /// <remarks>
        /// The text appears as a standard label with hand cursor on hover and opens the URL when clicked.
        /// Uses consistent text styling from theme settings with enhanced user interaction feedback.
        /// Text automatically wraps to new lines when it exceeds the available width, and the control height adjusts accordingly.
        /// This is a convenience wrapper around the action-based version for URL opening specifically.
        /// </remarks>
        [UsedImplicitly]
        public static bool ClickableTextWithCursor(string text, string url)
            => ClickableTextWithCursor(text, () => Application.OpenURL(url));
    }
}