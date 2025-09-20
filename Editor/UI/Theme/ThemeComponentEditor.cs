using System;
using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using CustomUtils.Runtime.UI.Theme.VertexGradient;
using UnityEditor;

namespace CustomUtils.Editor.UI.Theme
{
    [CustomEditor(typeof(ThemeComponent), true)]
    internal sealed class ThemeComponentEditor : EditorBase
    {
        private SolidColorDatabase SolidColorDatabase => SolidColorDatabase.Instance;
        private GradientColorDatabase GradientColorDatabase => GradientColorDatabase.Instance;

        private ThemeComponent _themeComponent;

        private bool _wasDarkTheme;

        protected override void InitializeEditor()
        {
            _themeComponent = target as ThemeComponent;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawColorTypeProperty();

            EditorGUILayout.Space();

            DrawThemeToggle();

            EditorGUILayout.Space();

            DrawColorSelector();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawColorTypeProperty()
        {
            var colorType =
                EditorStateControls.EnumField("Color Type", _themeComponent.CurrentColorType.Value);

            if (_themeComponent.CurrentColorType.Value == colorType)
                return;

            _themeComponent.CurrentColorType.Value = colorType;
            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }

        private void DrawThemeToggle()
        {
            EditorVisualControls.BeginPanel();

            var themeLabels = new[] { "Light Theme", "Dark Theme" };
            var selectedTheme = _wasDarkTheme ? 1 : 0;

            var newSelectedTheme = EditorStateControls.ToggleButtonGroup(themeLabels, selectedTheme);

            if (newSelectedTheme == selectedTheme)
                return;

            _wasDarkTheme = newSelectedTheme == 1;
            ThemeHandler.CurrentThemeType.Value = _wasDarkTheme ? ThemeType.Dark : ThemeType.Light;

            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }

        private void DrawColorSelector()
        {
            var colorNames = GetColorSelectorData(_themeComponent.CurrentColorType.Value);
            var currentColorName = _themeComponent.ColorNames[_themeComponent.CurrentColorType.Value];

            if (colorNames is null || colorNames.Count == 0)
            {
                EditorVisualControls.WarningBox("There is no colors in database.");
                return;
            }

            if (colorNames.Contains(currentColorName) is false || string.IsNullOrEmpty(currentColorName))
            {
                currentColorName = colorNames[0];
                UpdateColorFromName(currentColorName);
            }

            var selectedColorName =
                EditorStateControls.Dropdown("Name", currentColorName, colorNames);

            if (selectedColorName != currentColorName)
                UpdateColorFromName(selectedColorName);

            switch (_themeComponent.CurrentColorType.Value)
            {
                case ColorType.Gradient:
                    GradientColorDatabase.TryGetColorByName(currentColorName, out var gradient);
                    EditorVisualControls.GradientField("Preview", gradient);
                    DrawGradientDirectionDropdown();
                    break;

                case ColorType.Solid:
                    SolidColorDatabase.TryGetColorByName(currentColorName, out var color);
                    EditorVisualControls.ColorField("Preview", color);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawGradientDirectionDropdown()
        {
            var currentDirection = (int)_themeComponent.CurrentGradientDirection.Value;
            var selectedDirection = EditorStateControls.Dropdown(
                "Name",
                currentDirection,
                Enum.GetNames(typeof(GradientDirection)));

            if (selectedDirection == currentDirection)
                return;

            _themeComponent.CurrentGradientDirection.Value = (GradientDirection)selectedDirection;

            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }

        private List<string> GetColorSelectorData(ColorType colorType) =>
            colorType switch
            {
                ColorType.Gradient => GradientColorDatabase.GetColorNames(),
                ColorType.Solid => SolidColorDatabase.GetColorNames(),
                ColorType.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null)
            };

        private void UpdateColorFromName(string colorName)
        {
            _themeComponent.UpdateName(_themeComponent.CurrentColorType.Value, colorName);
            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }
    }
}