﻿using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
namespace CustomUtils.Editor.EditorTheme
{
    /// <summary>
    /// Enhanced GUI system for editor inspectors with automatic undo support.
    /// </summary>
    public sealed class EditorStateControls
    {
        private static ThemeEditorSettings Settings => ThemeEditorSettings.GetOrCreateSettings();
        private readonly Object _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorStateControls"/> class with a target object for undo support.
        /// </summary>
        /// <param name="target">The Unity object to track for undo operations. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public EditorStateControls([NotNull] Object target) => _target = target;

        /// <summary>
        /// Creates a color field with consistent styling and undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current color value.</param>
        /// <param name="useConsistentHeight">Whether to use a consistent height based on settings. Default is true.</param>
        /// <returns>The modified color value.</returns>
        public Color ColorField(string label, Color value, bool useConsistentHeight = true) =>
            HandleValueChange(label, value, () =>
                useConsistentHeight
                    ? EditorGUILayout.ColorField(label, value, GUILayout.Height(Settings.ColorFieldHeight))
                    : EditorGUILayout.ColorField(label, value));

        /// <summary>
        /// Creates a gradient field with consistent styling and undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current gradient value.</param>
        /// <param name="useConsistentHeight">Whether to use a consistent height based on settings. Default is true.</param>
        /// <returns>The modified gradient value.</returns>
        public Gradient GradientField(string label, Gradient value, bool useConsistentHeight = true) =>
            HandleValueChange(label, value, () =>
                useConsistentHeight
                    ? EditorGUILayout.GradientField(label, value, GUILayout.Height(Settings.ColorFieldHeight))
                    : EditorGUILayout.GradientField(label, value));

        /// <summary>
        /// Creates an object field with consistent styling and undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current object reference.</param>
        /// <param name="objectType">The type of object that can be assigned.</param>
        /// <param name="allowSceneObjects">Whether to allow scene objects to be assigned. Default is true.</param>
        /// <param name="useConsistentHeight">Whether to use a consistent height based on settings. Default is true.</param>
        /// <returns>The modified object reference.</returns>
        public Sprite SpriteField(string label, Sprite value, bool allowSceneObjects = false) =>
            HandleValueChange(label, value, () =>
                (Sprite)EditorGUILayout.ObjectField(label, value, typeof(Sprite), allowSceneObjects,
                    GUILayout.Height(Settings.ObjectFieldHeight)));

        public Object ObjectField(string label, Object value, Type type, bool allowSceneObjects = false,
            bool expandWidth = true) =>
            HandleValueChange(label, value, () =>
                EditorGUILayout.ObjectField(label, value, type, allowSceneObjects,
                    GUILayout.ExpandWidth(expandWidth)));

        /// <summary>
        /// Creates a float field with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current float value.</param>
        /// <returns>The modified float value.</returns>
        public float FloatField(string label, float value) =>
            HandleValueChange(label, value, () => EditorGUILayout.FloatField(label, value));

        /// <summary>
        /// Creates an int field with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current int value.</param>
        /// <returns>The modified int value.</returns>
        public int IntField(string label, int value) =>
            HandleValueChange(label, value, () => EditorGUILayout.IntField(label, value));

        /// <summary>
        /// Creates an enum field with undo support.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current enum value.</param>
        /// <returns>The modified enum value.</returns>
        public T EnumField<T>(string label, T value) where T : Enum =>
            HandleValueChange(label, value, () => (T)EditorGUILayout.EnumPopup(label, value));

        /// <summary>
        /// Creates an enum field with undo support.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The current enum value.</param>
        /// <returns>The modified enum value.</returns>
        public T EnumField<T>(T value) where T : Enum =>
            HandleValueChange(nameof(T), value, () => (T)EditorGUILayout.EnumPopup(value));

        /// <summary>
        /// Creates a property field with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the property field.</param>
        /// <param name="property">The serialized property to modify.</param>
        /// <param name="includeChildren">Whether to include children of the property. Default is false.</param>
        /// <returns>True if the property was modified, false otherwise.</returns>
        public bool PropertyField(SerializedProperty property, string label, bool includeChildren = false)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(property, new GUIContent(label), includeChildren);

            if (EditorGUI.EndChangeCheck() is false)
                return false;

            Undo.RecordObject(_target, $"Change {label}");
            return true;
        }

        /// <summary>
        /// Creates a property field with undo support using the property's built-in label.
        /// </summary>
        /// <param name="property">The serialized property to modify.</param>
        /// <param name="includeChildren">Whether to include children of the property. Default is false.</param>
        /// <returns>True if the property was modified, false otherwise.</returns>
        public bool PropertyField(SerializedProperty property, bool includeChildren = false)
            => PropertyField(property, property.displayName, includeChildren);

        /// <summary>
        /// Creates a dropdown with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the dropdown.</param>
        /// <param name="selectedIndex">The currently selected index.</param>
        /// <param name="options">Array of option strings to display.</param>
        /// <param name="indented">Whether to indent the dropdown. Default is true.</param>
        /// <returns>The index of the selected option.</returns>
        public int Dropdown(string label, int selectedIndex, string[] options, bool indented = true)
        {
            var dropdownStyle = EditorVisualControls.CreateTextStyle(
                EditorStyles.popup,
                Settings.DropdownFontSize,
                Settings.DropdownFontStyle);

            var originalIndent = EditorGUI.indentLevel;
            if (indented)
                EditorGUI.indentLevel++;

            var result = HandleValueChange(label, selectedIndex, () =>
                EditorGUILayout.Popup(
                    label,
                    selectedIndex,
                    options,
                    dropdownStyle,
                    GUILayout.Height(Settings.DropdownHeight)
                ));

            EditorGUI.indentLevel = originalIndent;
            return result;
        }

        /// <summary>
        /// Creates a toggle button with undo support.
        /// </summary>
        /// <param name="label">The text to display on the button.</param>
        /// <param name="isSelected">Whether the button is currently selected.</param>
        /// <param name="highlightColor">Optional color to use when the button is selected. If null, use the default from settings.</param>
        /// <returns>The newly selected state of the button.</returns>
        public bool ToggleButton(string label, bool isSelected, Color? highlightColor = null)
        {
            var buttonStyle = EditorVisualControls.CreateTextStyle(
                GUI.skin.button,
                Settings.ButtonFontSize,
                Settings.ButtonFontStyle);

            var originalBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = isSelected
                ? highlightColor ?? Settings.ButtonHighlightColor
                : Settings.ButtonBackgroundColor;

            EditorGUI.BeginChangeCheck();
            var clicked = GUILayout.Button(label, buttonStyle, GUILayout.Height(Settings.ButtonHeight));

            GUI.backgroundColor = originalBackgroundColor;

            if (EditorGUI.EndChangeCheck() is false || clicked is false)
                return isSelected;

            Undo.RecordObject(_target, $"Toggle {label}");
            return isSelected is false;
        }

        /// <summary>
        /// Creates a group of toggle buttons where one can be selected at a time.
        /// </summary>
        /// <param name="labels">Array of button labels to display.</param>
        /// <param name="selectedIndex">The currently selected button index.</param>
        /// <returns>The index of the newly selected button.</returns>
        public int ToggleButtonGroup(string[] labels, int selectedIndex)
        {
            EditorGUILayout.BeginHorizontal();

            var newIndex = selectedIndex;

            for (var i = 0; i < labels.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                var isSelected = selectedIndex == i;
                var newIsSelected = ToggleButton(labels[i], isSelected);

                if (EditorGUI.EndChangeCheck() && newIsSelected && isSelected is false)
                    newIndex = i;
            }

            EditorGUILayout.EndHorizontal();

            return newIndex;
        }

        /// <summary>
        /// Creates a toggle with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the toggle.</param>
        /// <param name="value">The current toggle state.</param>
        /// <returns>The new toggle state.</returns>
        public bool Toggle(string label, bool value) =>
            HandleValueChange(label, value, () => EditorGUILayout.Toggle(label, value));

        /// <summary>
        /// Creates two mutually exclusive toggles with undo support.
        /// </summary>
        /// <remarks>
        /// When one toggle is enabled, the other is automatically disabled.
        /// At least one toggle will always remain enabled.
        /// </remarks>
        /// <param name="toggle1">Reference to the first toggle's state.</param>
        /// <param name="toggle2">Reference to the second toggle's state.</param>
        /// <param name="label1">The label for the first toggle.</param>
        /// <param name="label2">The label for the second toggle.</param>
        public void ExclusiveToggles(ref bool toggle1, ref bool toggle2, string label1, string label2)
        {
            EditorGUI.BeginChangeCheck();

            var oldToggle1 = toggle1;
            var oldToggle2 = toggle2;

            EditorGUILayout.BeginHorizontal();

            var newToggle1 = EditorGUILayout.Toggle(new GUIContent(label1), toggle1);
            var newToggle2 = EditorGUILayout.Toggle(new GUIContent(label2), toggle2);

            EditorGUILayout.EndHorizontal();

            if (newToggle1 == oldToggle1 && newToggle2 == oldToggle2)
                return;

            if (EditorGUI.EndChangeCheck() is false)
                return;

            Undo.RecordObject(_target, $"Change {label1}/{label2} Selection");

            toggle1 = newToggle1;
            toggle2 = newToggle2;

            switch (newToggle1)
            {
                case true when newToggle2:
                    if (oldToggle1 == false)
                        toggle2 = false;
                    else
                        toggle1 = false;
                    break;
                case false when newToggle2 == false:
                    if (oldToggle1)
                        toggle1 = true;
                    else
                        toggle2 = true;
                    break;
            }
        }

        /// <summary>
        /// Creates a multi-line text area with undo support.
        /// </summary>
        /// <param name="label">The label to display above the text area.</param>
        /// <param name="value">The current text content.</param>
        /// <returns>The modified text content.</returns>
        public string TextArea(string label, string value)
        {
            EditorGUILayout.LabelField(label);
            return HandleValueChange(label, value, () => EditorGUILayout.TextArea(value));
        }

        /// <summary>
        /// Creates a single-line text field with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the field.</param>
        /// <param name="value">The current text content.</param>
        /// <returns>The modified text content.</returns>
        public string TextField(string label, string value) =>
            HandleValueChange(label, value, () => EditorGUILayout.TextField(label, value));

        /// <summary>
        /// Creates a float slider with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the slider.</param>
        /// <param name="value">The current float value.</param>
        /// <param name="leftValue">The minimum value (left end of slider).</param>
        /// <param name="rightValue">The maximum value (right end of slider).</param>
        /// <returns>The modified float value.</returns>
        public float Slider(string label, float value, float leftValue, float rightValue) =>
            HandleValueChange(label, value, () => EditorGUILayout.Slider(label, value, leftValue, rightValue));

        /// <summary>
        /// Creates an int slider with undo support.
        /// </summary>
        /// <param name="label">The label to display next to the slider.</param>
        /// <param name="value">The current int value.</param>
        /// <param name="leftValue">The minimum value (left end of slider).</param>
        /// <param name="rightValue">The maximum value (right end of slider).</param>
        /// <returns>The modified int value.</returns>
        public int IntSlider(string label, int value, int leftValue, int rightValue) =>
            HandleValueChange(label, value, () => EditorGUILayout.IntSlider(label, value, leftValue, rightValue));

        /// <summary>
        /// Helper method that handles change detection and undo recording for GUI controls.
        /// </summary>
        /// <typeparam name="T">The type of value being modified.</typeparam>
        /// <param name="label">The label used for the undo operation.</param>
        /// <param name="currentValue">The current value before any changes.</param>
        /// <param name="guiMethod">The function that creates the GUI control and returns its new value.</param>
        /// <returns>The current value if unchanged, or the new value if modified.</returns>
        private T HandleValueChange<T>(string label, T currentValue, Func<T> guiMethod)
        {
            EditorGUI.BeginChangeCheck();
            var newValue = guiMethod();
            if (EditorGUI.EndChangeCheck() is false)
                return currentValue;

            Undo.RecordObject(_target, $"Change {label}");
            return newValue;
        }
    }
}