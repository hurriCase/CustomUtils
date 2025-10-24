// using System;
// using CustomUtils.Editor.Scripts.CustomEditorUtilities;
// using CustomUtils.Editor.Scripts.Extensions;
// using CustomUtils.Runtime.Localization;
// using TMPro;
// using UnityEditor;
// using UnityEngine;
// using ZLinq;
//
// namespace CustomUtils.Editor.Scripts.Localization
// {
//     [CanEditMultipleObjects]
//     [CustomEditor(typeof(LocalizedTextMeshPro))]
//     internal sealed class LocalizedTextMeshProEditor : EditorBase
//     {
//         private TextMeshProUGUI _textComponent;
//         private SystemLanguage _selectedLanguage;
//
//         private SerializedProperty _localizationKeyProperty;
//         private SerializedProperty _textProperty;
//
//         protected override void InitializeEditor()
//         {
//             if (serializedObject.TryGetComponent(out _textComponent) is false)
//                 return;
//
//             _localizationKeyProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.LocalizationKey));
//             _textProperty = serializedObject.FindField(nameof(LocalizedTextMeshPro.Text));
//
//             _selectedLanguage = LocalizationDatabase.Instance.DefaultLanguage;
//         }
//
//         public override void OnInspectorGUI()
//         {
//             serializedObject.Update();
//
//             EditorStateControls.PropertyField(_localizationKeyProperty);
//
//             var guidProperty = _localizationKeyProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
//             if (string.IsNullOrEmpty(guidProperty.stringValue) is false)
//             {
//                 DrawLanguageSelection();
//                 DrawLocalizedContent();
//             }
//
//             EditorStateControls.PropertyField(_textProperty);
//
//             serializedObject.ApplyModifiedProperties();
//         }
//
//         private void DrawLanguageSelection()
//         {
//             EditorGUI.BeginChangeCheck();
//
//             var availableLanguages = LocalizationController.GetAllLanguages();
//             if (availableLanguages == null || availableLanguages.Length == 0)
//             {
//                 EditorVisualControls.WarningBox("No languages found in localization data.");
//                 return;
//             }
//
//             var languageStrings = availableLanguages.AsValueEnumerable()
//                 .Select(static lang => lang.ToString()).ToArray();
//
//             var currentIndex = Array.IndexOf(languageStrings, _selectedLanguage.ToString());
//             if (currentIndex == -1)
//                 currentIndex = 0;
//
//             var newIndex = EditorStateControls.Dropdown("Preview Language", currentIndex, languageStrings);
//             _selectedLanguage = availableLanguages[newIndex];
//
//             if (EditorGUI.EndChangeCheck())
//                 ApplyLocalizedText();
//         }
//
//         private void DrawLocalizedContent()
//         {
//             var guidProperty = _localizationKeyProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
//
//             if (LocalizationRegistry.Instance.TryGetEntry(guidProperty.stringValue, out var entry) is false)
//             {
//                 EditorVisualControls.WarningBox("Localization entry not found in registry.");
//                 return;
//             }
//
//             var availableLanguages = LocalizationController.GetAllLanguages();
//             if (availableLanguages == null || availableLanguages.Length == 0)
//                 return;
//
//             foreach (var language in availableLanguages)
//                 DrawLanguageRow(entry, language);
//         }
//
//         private void DrawLanguageRow(LocalizationEntry entry, SystemLanguage language)
//         {
//             using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();
//
//             EditorVisualControls.LabelField(language.ToString(), GUILayout.Width(100));
//
//             var hasTranslation = entry.TryGetTranslation(language, out var localizedText);
//             var displayText = hasTranslation ? localizedText : "[Empty]";
//
//             if (EditorVisualControls.Button(displayText, GUI.skin.textField))
//                 ApplyPreviewText(localizedText);
//         }
//
//         private void ApplyPreviewText(string text)
//         {
//             if (string.IsNullOrEmpty(text))
//                 return;
//
//             Undo.RecordObject(_textComponent, "Apply Localized Text Preview");
//             _textComponent.text = text;
//             EditorUtility.SetDirty(_textComponent);
//         }
//
//         private void ApplyLocalizedText()
//         {
//             var guidProperty = _localizationKeyProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
//
//             if (string.IsNullOrEmpty(guidProperty.stringValue))
//                 return;
//
//             if (LocalizationRegistry.Instance.TryGetEntry(guidProperty.stringValue, out var entry) is false)
//                 return;
//
//             if (entry.TryGetTranslation(_selectedLanguage, out var localizedText) is false)
//                 return;
//
//             Undo.RecordObject(_textComponent, "Apply Localized Text");
//             _textComponent.text = localizedText;
//             EditorUtility.SetDirty(_textComponent);
//         }
//     }
// }