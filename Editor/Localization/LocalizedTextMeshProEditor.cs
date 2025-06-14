using UnityEditor;
using CustomUtils.Runtime.Localization;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
using TMPro;

namespace CustomUtils.Editor.Localization
{
    [CustomEditor(typeof(LocalizedTextMeshPro))]
    internal sealed class LocalizedTextMeshProEditor : EditorBase
    {
        private TextMeshProUGUI _textComponent;
        private string _selectedLanguage;

        private SerializedProperty _localizationKeyProperty;

        protected override void InitializeEditor()
        {
            if (target && target is LocalizedTextMeshPro localizedText)
                _textComponent = localizedText.TextComponent;

            _localizationKeyProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.LocalizationKey));

            _selectedLanguage = LocalizationSettings.Instance.DefaultLanguage;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawKeySelection();

            DrawLanguageSelection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeySelection()
        {
            var availableKeys = LocalizationController.GetAllKeys();

            if (availableKeys == null || availableKeys.Length == 0)
            {
                EditorVisualControls.WarningBox("No localization data found. Check LocalizationSettings.");
                DrawDefaultInspector();
                return;
            }

            var newLocalizationKey = EditorStateControls
                .Dropdown("Localization Key", _localizationKeyProperty.stringValue, availableKeys);

            if (newLocalizationKey == _localizationKeyProperty.stringValue)
                return;

            _localizationKeyProperty.stringValue = newLocalizationKey;
            serializedObject.ApplyModifiedProperties();

            ApplyLocalizedText();
        }

        private void DrawLanguageSelection()
        {
            EditorGUI.BeginChangeCheck();

            var availableLanguages = LocalizationController.GetAllLanguages();
            _selectedLanguage =
                EditorStateControls.Dropdown("Language", _selectedLanguage, availableLanguages);

            if (EditorGUI.EndChangeCheck())
                ApplyLocalizedText();
        }

        private void ApplyLocalizedText()
        {
            if (string.IsNullOrEmpty(_localizationKeyProperty.stringValue))
                return;

            var localizedText =
                LocalizationController.GetLocalizedText(_localizationKeyProperty.stringValue, _selectedLanguage);

            Undo.RecordObject(_textComponent, "Apply Localized Text");
            _textComponent.text = localizedText;
            EditorUtility.SetDirty(_textComponent);

            ApplyLocalizedFont(_selectedLanguage);
        }

        private void ApplyLocalizedFont(string language)
        {
            if (LocalizationController.TryGetFontForLanguage(language, out var fontMapping) is false ||
                !fontMapping.Font)
                return;

            Undo.RecordObject(_textComponent, "Apply Localized Font");
            _textComponent.font = fontMapping.Font;
            EditorUtility.SetDirty(_textComponent);
        }
    }
}