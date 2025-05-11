using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedMember.Global
namespace CustomUtils.Editor.EditorTheme
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for all custom editor windows with enhanced GUI capabilities.
    /// </summary>
    /// <remarks>
    /// Provides common functionality including section management, state persistence,
    /// and access to enhanced GUI controls. Use this as a foundation for creating
    /// custom editor windows with consistent styling and behavior.
    /// </remarks>
    public abstract class WindowBase : EditorWindow
    {
        /// <summary>
        /// Access to the enhanced GUI system with automatic undo support.
        /// </summary>
        /// <remarks>
        /// This property lazily initializes the EditorGUIExtensions instance
        /// when first accessed, passing the window instance for undo operations.
        /// </remarks>
        protected EditorStateControls EditorStateControls => _editorGUI ??= new EditorStateControls(this);
        private EditorStateControls _editorGUI;

        private const string PrefPrefix = "WindowBase_";
        private readonly Dictionary<string, bool> _foldoutStates = new();

        /// <summary>
        /// Called when the window is enabled or opened.
        /// </summary>
        /// <remarks>
        /// Loads saved window preferences and initializes state. Override this method in derived classes
        /// to perform custom initialization, but be sure to call the base implementation first.
        /// </remarks>
        private void OnEnable()
        {
            _foldoutStates.Clear();

            InitializeWindow();
        }

        /// <summary>
        /// Called when the window is disabled or closed.
        /// </summary>
        /// <remarks>
        /// Saves all window states to EditorPrefs. Override this method in derived classes
        /// to perform additional cleanup, but be sure to call the base implementation last.
        /// </remarks>
        private void OnDisable()
        {
            CleanupWindow();
            SaveWindowPreferences();
        }

        /// <summary>
        /// Initializes the window. This method is called during OnEnable and should be overridden
        /// by derived classes to perform any custom initialization.
        /// </summary>
        protected virtual void InitializeWindow() { }

        /// <summary>
        /// Performs window cleanup tasks. This method is called during OnDisable and should be overridden
        /// by derived classes to perform any custom cleanup operations.
        /// </summary>
        protected virtual void CleanupWindow() { }

        /// <summary>
        /// Called when the GUI should be drawn.
        /// </summary>
        /// <remarks>
        /// This implementation sets up the necessary styling and calls DrawWindowContent()
        /// for the actual content rendering.
        /// </remarks>
        private void OnGUI()
        {
            DrawWindowContent();
        }

        /// <summary>
        /// Override this method in derived classes to draw the window's content.
        /// </summary>
        /// <remarks>
        /// This is the primary extension point for custom windows. When overriding this method,
        /// use the <see cref="DrawFoldoutSection"/> and <see cref="DrawSection"/> methods to create
        /// consistently styled UI sections for your custom controls.
        /// </remarks>
        protected abstract void DrawWindowContent();

        /// <summary>
        /// Draws a custom section with a foldout header.
        /// </summary>
        /// <remarks>
        /// The foldout state is automatically saved between editor sessions.
        /// Use this method to organize related controls in collapsible sections.
        /// </remarks>
        /// <param name="title">Title of the section to display in the header.</param>
        /// <param name="drawContent">Action to execute when drawing the section content.</param>
        protected void DrawFoldoutSection(string title, Action drawContent)
        {
            _foldoutStates.TryAdd(title, true);

            var foldout = _foldoutStates[title];
            EditorVisualControls.DrawBoxWithFoldout(title, ref foldout, drawContent);
            _foldoutStates[title] = foldout;
        }

        /// <summary>
        /// Draws a custom section with a fixed header (no foldout).
        /// </summary>
        /// <remarks>
        /// Use this method to organize related controls in non-collapsible sections.
        /// </remarks>
        /// <param name="title">Title of the section to display in the header.</param>
        /// <param name="drawContent">Action to execute when drawing the section content.</param>
        protected void DrawSection(string title, Action drawContent)
        {
            EditorVisualControls.DrawBoxedSection(title, drawContent);
        }

        /// <summary>
        /// Saves window preferences to EditorPrefs.
        /// </summary>
        /// <remarks>
        /// Persists foldout states and other window-specific preferences.
        /// </remarks>
        private void SaveWindowPreferences()
        {
            var windowTypeName = GetType().Name;

            foreach (var foldoutState in _foldoutStates)
            {
                EditorPrefs.SetBool(
                    $"{PrefPrefix}{windowTypeName}_Foldout_{SanitizeKey(foldoutState.Key)}",
                    foldoutState.Value
                );
            }
        }

        /// <summary>
        /// Sanitizes a string key for use in EditorPrefs.
        /// </summary>
        /// <param name="key">The key to sanitize.</param>
        /// <param name="reverse">Whether to reverse the sanitization process.</param>
        /// <returns>A sanitized string suitable for use as an EditorPrefs key.</returns>
        private string SanitizeKey(string key, bool reverse = false)
            => reverse ? key.Replace("_", " ").Replace("__", ".") : key.Replace(" ", "_").Replace(".", "__");
    }
}