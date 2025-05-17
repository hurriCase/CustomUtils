﻿using System;
using System.Collections.Generic;
using UnityEditor;

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

        /// <summary>
        /// Access to the progress tracker for handling long-running operations.
        /// </summary>
        protected EditorProgressTracker ProgressTracker => _progressTracker ??= new EditorProgressTracker();
        private EditorProgressTracker _progressTracker;

        private const string PrefPrefix = "WindowBase_";
        private readonly Dictionary<string, bool> _foldoutStates = new();

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
        protected static void DrawSection(string title, Action drawContent)
            => EditorVisualControls.DrawBoxedSection(title, drawContent);

        /// <summary>
        /// Updates the progress information and triggers a repaint of the window
        /// </summary>
        /// <param name="operation">Name of the operation</param>
        /// <param name="info">Additional information</param>
        /// <param name="progress">Progress value between 0 and 1</param>
        protected void UpdateProgress(string operation, string info, float progress)
            => ProgressTracker.UpdateProgress(operation, info, progress);

        /// <summary>
        /// Draws the progress bar if an operation is in progress
        /// </summary>
        protected void DrawProgressIfNeeded() => ProgressTracker.DrawProgressIfNeeded();

        /// <summary>
        /// Completes the current operation and resets progress tracking
        /// </summary>
        protected void CompleteOperation(string completeInfo = null) => ProgressTracker.CompleteOperation(completeInfo);

        private void OnEnable()
        {
            _foldoutStates.Clear();

            InitializeWindow();
        }

        private void OnDisable()
        {
            _progressTracker.Dispose();

            CleanupWindow();
            SaveWindowPreferences();
        }

        private void OnGUI()
        {
            DrawWindowContent();
        }

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

        private string SanitizeKey(string key, bool reverse = false)
            => reverse ? key.Replace("_", " ").Replace("__", ".") : key.Replace(" ", "_").Replace(".", "__");
    }
}