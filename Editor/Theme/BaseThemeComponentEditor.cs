using System;
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

        private SerializedProperty _sharedColorNameProperty;
        private SerializedProperty _solidColorNameProperty;
        private SerializedProperty _gradientColorNameProperty;

        private int _solidColorIndex;
        private int _gradientColorIndex;
        private int _sharedColorIndex;

        private bool _previewDarkTheme;

        private int _newIndex;

        protected override void InitializeEditor()
        {
            _themeComponent = target as IBaseThemeComponent;

            if (_themeComponent == null)
                return;

            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeSharedColor), ColorType.Shared,
                out _sharedColorNameProperty, out _sharedColorIndex);

            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeSolidColor), ColorType.SolidColor,
                out _solidColorNameProperty, out _solidColorIndex);

            InitializeColorProperty(nameof(IBaseThemeComponent.ThemeGradientColor), ColorType.Gradient,
                out _gradientColorNameProperty, out _gradientColorIndex);
        }

        private void InitializeColorProperty(string propertyName, ColorType colorType,
            out SerializedProperty colorNameProperty, out int colorIndex)
        {
            var colorProperty = serializedObject.FindField(propertyName);
            colorNameProperty = colorProperty.FindFieldRelative(nameof(IThemeColor.Name));
            var currentColorIndex = ThemeColorDatabase.GetColorIndexByName(colorNameProperty.stringValue, colorType);
            colorIndex = currentColorIndex == -1 ? 0 : currentColorIndex;
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
            UpdateColorAndPreview();

            EditorUtility.SetDirty(target);
        }

        private void DrawThemeToggle()
        {
            EditorVisualControls.BeginPanel();

            string[] themeLabels = { "Light Theme", "Dark Theme" };
            var selectedTheme = _previewDarkTheme ? 1 : 0;

            var newSelectedTheme = EditorStateControls.ToggleButtonGroup(themeLabels, selectedTheme);

            if (newSelectedTheme == selectedTheme)
                return;

            _previewDarkTheme = newSelectedTheme == 1;
            ThemeHandler.CurrentThemeType = _previewDarkTheme ? ThemeType.Dark : ThemeType.Light;

            _themeComponent.OnApplyColor();
            EditorUtility.SetDirty(target);
        }

        private void DrawColorSelector()
        {
            var (names, index) = GetColorSelectorData(_themeComponent.ColorType);

            using var changeCheck = EditorVisualControls.BeginBoxedSection("Color");

            _newIndex = EditorStateControls.Dropdown(nameof(_themeComponent.ColorType), index, names);

            if (_newIndex != index)
                UpdateColorAndPreview();

            switch (_themeComponent.ColorType)
            {
                case ColorType.Shared:
                    EditorVisualControls.ColorField("Preview", _themeComponent.ThemeSharedColor.Color);
                    break;

                case ColorType.SolidColor:
                    var previewSolidColor = ThemeHandler.CurrentThemeType == ThemeType.Light
                        ? _themeComponent.ThemeSolidColor.LightThemeColor
                        : _themeComponent.ThemeSolidColor.DarkThemeColor;

                    EditorVisualControls.ColorField("Preview", previewSolidColor);
                    break;

                case ColorType.Gradient:
                    var previewGradient = ThemeHandler.CurrentThemeType == ThemeType.Light
                        ? _themeComponent.ThemeGradientColor.LightThemeColor
                        : _themeComponent.ThemeGradientColor.DarkThemeColor;

                    EditorVisualControls.GradientField("Preview", previewGradient);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private (string[], int) GetColorSelectorData(ColorType colorType) =>
            colorType switch
            {
                ColorType.Shared => (ThemeColorDatabase.GetColorNames<ThemeSharedColor>(), _sharedColorIndex),
                ColorType.SolidColor => (ThemeColorDatabase.GetColorNames<ThemeSolidColor>(), _solidColorIndex),
                ColorType.Gradient => (ThemeColorDatabase.GetColorNames<ThemeGradientColor>(), _gradientColorIndex),
                _ => throw new ArgumentOutOfRangeException()
            };

        private void UpdateColorAndPreview()
        {
            switch (_themeComponent.ColorType)
            {
                case ColorType.Shared:
                    var sharedColor = ThemeColorDatabase.SharedColor[_newIndex];
                    _themeComponent.ThemeSharedColor = sharedColor;
                    _sharedColorIndex = _newIndex;
                    _sharedColorNameProperty.stringValue = sharedColor.Name;
                    break;

                case ColorType.SolidColor:
                    var solidColor = ThemeColorDatabase.SolidColors[_newIndex];
                    _themeComponent.ThemeSolidColor = solidColor;
                    _solidColorIndex = _newIndex;
                    _solidColorNameProperty.stringValue = solidColor.Name;
                    break;

                case ColorType.Gradient:
                    var gradientColor = ThemeColorDatabase.GradientColors[_newIndex];
                    _themeComponent.ThemeGradientColor = gradientColor;
                    _gradientColorIndex = _newIndex;
                    _gradientColorNameProperty.stringValue = gradientColor.Name;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _themeComponent.OnApplyColor();
            EditorUtility.SetDirty(target);
        }
    }
}