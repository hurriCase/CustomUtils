using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEditor;

namespace CustomUtils.Editor.UI.Theme
{
    [CustomEditor(typeof(ThemeHandler))]
    internal sealed class ThemeHandlerEditor : EditorBase
    {
        private ThemeHandler _themeHandler;
        private bool _editingLightTheme = true;

        protected override void InitializeEditor()
        {
            _themeHandler = target as ThemeHandler;
        }

        protected override void DrawCustomSections()
        {
            EditorVisualControls.H1Label("Current Theme");

            EditorVisualControls.DrawPanel(DrawThemeSelection);
        }

        private void DrawThemeSelection()
        {
            string[] themeLabels = { "Light Theme", "Dark Theme" };
            var selectedTheme = _editingLightTheme ? 0 : 1;

            var newSelectedTheme = EditorStateControls.ToggleButtonGroup(themeLabels, selectedTheme);

            if (newSelectedTheme == selectedTheme)
                return;

            _editingLightTheme = newSelectedTheme == 0;

            var targetTheme = _editingLightTheme ? ThemeType.Light : ThemeType.Dark;

            if (_themeHandler && _themeHandler.CurrentThemeType != targetTheme)
                _themeHandler.CurrentThemeType = targetTheme;
        }
    }
}