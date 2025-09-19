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
        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;
        private ThemeHandler ThemeHandler => ThemeHandler.Instance;

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
            var (colorNames, currentColorName) = GetColorSelectorData(_themeComponent.CurrentColorType.Value);

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
                EditorStateControls.Dropdown(nameof(IThemeColor.Name), currentColorName, colorNames);

            if (selectedColorName != currentColorName)
                UpdateColorFromName(selectedColorName);

            switch (_themeComponent.CurrentColorType.Value)
            {
                case ColorType.Gradient:
                    var previewGradient = ThemeHandler.CurrentThemeType.Value == ThemeType.Light
                        ? _themeComponent.ThemeGradientColor.LightThemeColor
                        : _themeComponent.ThemeGradientColor.DarkThemeColor;

                    EditorVisualControls.GradientField("Preview", previewGradient);
                    DrawGradientDirectionDropdown();
                    break;

                case ColorType.Shared:
                    EditorVisualControls.ColorField("Preview", _themeComponent.ThemeSharedColor.Color);
                    break;

                case ColorType.Solid:
                    var previewSolidColor = ThemeHandler.CurrentThemeType.Value == ThemeType.Light
                        ? _themeComponent.ThemeSolidColor.LightThemeColor
                        : _themeComponent.ThemeSolidColor.DarkThemeColor;

                    EditorVisualControls.ColorField("Preview", previewSolidColor);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawGradientDirectionDropdown()
        {
            var currentDirection = (int)_themeComponent.CurrentGradientDirection.Value;
            var selectedDirection = EditorStateControls.Dropdown(
                nameof(IThemeColor.Name),
                currentDirection,
                Enum.GetNames(typeof(GradientDirection)));

            if (selectedDirection == currentDirection)
                return;

            _themeComponent.CurrentGradientDirection.Value = (GradientDirection)selectedDirection;

            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }

        private (List<string>, string) GetColorSelectorData(ColorType colorType) =>
            colorType switch
            {
                ColorType.Gradient => (
                    ThemeColorDatabase.GetColorNames<ThemeGradientColor>(),
                    _themeComponent.ThemeGradientColorName
                ),
                ColorType.Shared => (
                    ThemeColorDatabase.GetColorNames<ThemeSharedColor>(),
                    _themeComponent.ThemeSharedColorName
                ),
                ColorType.Solid => (
                    ThemeColorDatabase.GetColorNames<ThemeSolidColor>(),
                    _themeComponent.ThemeSolidColorName
                ),
                ColorType.None => (null, null),
                _ => throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null)
            };

        private void UpdateColorFromName(string colorName)
        {
            switch (_themeComponent.CurrentColorType.Value)
            {
                case ColorType.Solid:
                    _themeComponent.ThemeSolidColorName = colorName;
                    break;

                case ColorType.Gradient:
                    _themeComponent.ThemeGradientColorName = colorName;
                    break;

                case ColorType.Shared:
                    _themeComponent.ThemeSharedColorName = colorName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _themeComponent.ApplyColor();
            EditorUtility.SetDirty(target);
        }
    }
}