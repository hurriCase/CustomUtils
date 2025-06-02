using System;
using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEditor;

namespace CustomUtils.Editor.Theme
{
    [CustomEditor(typeof(BaseThemeComponent<>), true)]
    internal sealed class BaseThemeComponentEditor : EditorBase
    {
        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;
        private ThemeHandler ThemeHandler => ThemeHandler.Instance;

        private IBaseThemeComponent _themeComponent;

        private SerializedProperty _gradientColorNameProperty;
        private SerializedProperty _sharedColorNameProperty;
        private SerializedProperty _solidColorNameProperty;

        private bool _wasDarkTheme;

        protected override void InitializeEditor()
        {
            _themeComponent = target as IBaseThemeComponent;

            if (_themeComponent == null)
                return;

            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeGradientColor), out _gradientColorNameProperty);
            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeSharedColor), out _sharedColorNameProperty);
            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeSolidColor), out _solidColorNameProperty);
        }

        private void InitializeColorProperty(string propertyName, out SerializedProperty colorNameProperty)
        {
            var colorProperty = serializedObject.FindField(propertyName);
            colorNameProperty = colorProperty.FindFieldRelative(nameof(IThemeColor.Name));
        }

        protected override void DrawCustomSections()
        {
            DrawFoldoutSection("Theme Settings", () =>
            {
                DrawColorTypeProperty();
                DrawThemeToggle();
                DrawColorSelector();
            });
        }

        private void DrawColorTypeProperty()
        {
            var colorType = EditorStateControls.EnumField("Color Type", _themeComponent.ColorType);

            if (_themeComponent.ColorType == colorType)
                return;

            _themeComponent.ColorType = colorType;
            _themeComponent.OnApplyColor();

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
            ThemeHandler.CurrentThemeType = _wasDarkTheme ? ThemeType.Dark : ThemeType.Light;

            _themeComponent.OnApplyColor();
            EditorUtility.SetDirty(target);
        }

        private void DrawColorSelector()
        {
            var (colorNames, currentColorName) = GetColorSelectorData(_themeComponent.ColorType);

            using var changeCheck = EditorVisualControls.BeginBoxedSection("Color");

            if (colorNames is null || colorNames.Count == 0 || colorNames.Contains(currentColorName) is false)
            {
                EditorVisualControls.LabelField("There is no colors in database or color name is invalid.");
                return;
            }

            var selectedColorName =
                EditorStateControls.Dropdown(nameof(IThemeColor.Name), currentColorName, colorNames);

            if (selectedColorName != currentColorName)
                UpdateColorFromName(selectedColorName);

            switch (_themeComponent.ColorType)
            {
                case ColorType.Gradient:
                    var previewGradient = ThemeHandler.CurrentThemeType == ThemeType.Light
                        ? _themeComponent.ThemeGradientColor.LightThemeColor
                        : _themeComponent.ThemeGradientColor.DarkThemeColor;

                    EditorVisualControls.GradientField("Preview", previewGradient);
                    break;

                case ColorType.Shared:
                    EditorVisualControls.ColorField("Preview", _themeComponent.ThemeSharedColor.Color);
                    break;

                case ColorType.Solid:
                    var previewSolidColor = ThemeHandler.CurrentThemeType == ThemeType.Light
                        ? _themeComponent.ThemeSolidColor.LightThemeColor
                        : _themeComponent.ThemeSolidColor.DarkThemeColor;

                    EditorVisualControls.ColorField("Preview", previewSolidColor);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private (List<string>, string) GetColorSelectorData(ColorType colorType) =>
            colorType switch
            {
                ColorType.Gradient => (ThemeColorDatabase.GetColorNames<ThemeGradientColor>(),
                    _themeComponent.ThemeGradientColor?.Name ?? string.Empty),
                ColorType.Shared => (ThemeColorDatabase.GetColorNames<ThemeSharedColor>(),
                    _themeComponent.ThemeSharedColor?.Name ?? string.Empty),
                ColorType.Solid => (ThemeColorDatabase.GetColorNames<ThemeSolidColor>(),
                    _themeComponent.ThemeSolidColor?.Name ?? string.Empty),
                _ => throw new ArgumentOutOfRangeException()
            };

        private void UpdateColorFromName(string colorName)
        {
            switch (_themeComponent.ColorType)
            {
                case ColorType.Gradient:
                    if (ThemeColorDatabase.TryGetColorByName<ThemeGradientColor>(colorName, out var gradientColor))
                    {
                        _themeComponent.ThemeGradientColor = gradientColor;
                        _gradientColorNameProperty.stringValue = gradientColor.Name;
                    }

                    break;

                case ColorType.Shared:
                    if (ThemeColorDatabase.TryGetColorByName<ThemeSharedColor>(colorName, out var sharedColor))
                    {
                        _themeComponent.ThemeSharedColor = sharedColor;
                        _sharedColorNameProperty.stringValue = sharedColor.Name;
                    }

                    break;
                case ColorType.Solid:
                    if (ThemeColorDatabase.TryGetColorByName<ThemeSolidColor>(colorName, out var solidColor))
                    {
                        _themeComponent.ThemeSolidColor = solidColor;
                        _solidColorNameProperty.stringValue = solidColor.Name;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            _themeComponent.OnApplyColor();
            EditorUtility.SetDirty(target);
        }
    }
}