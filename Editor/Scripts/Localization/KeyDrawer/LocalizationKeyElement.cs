using CustomUtils.Editor.Scripts.Localization.LocalizationSelector;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization.KeyDrawer
{
    internal sealed class LocalizationKeyElement : VisualElement
    {
        private readonly SerializedProperty _guidProperty;
        private readonly Foldout _translationsContainer;

        private DropdownField _keyDropdown;

        private LocalizationEntry _selectedEntry;

        internal LocalizationKeyElement(SerializedProperty guidProperty, string label)
        {
            _guidProperty = guidProperty;

            _translationsContainer = new Foldout { text = label };

            SetupKeyField(label);

            SetupToggle();

            Add(_translationsContainer);
        }

        internal void UpdateLocalizationKey(LocalizationEntry entry)
        {
            _selectedEntry = entry;

            _keyDropdown.value = entry.Key;

            foreach (var (systemLanguage, localization) in entry.Translations)
            {
                var translation = new TextField
                {
                    isReadOnly = true,
                    label = systemLanguage.ToString(),
                    value = localization
                };

                _translationsContainer.Add(translation);
            }
        }

        private void ShowKeySelectionWindow()
        {
            LocalizationSelectorWindow.ShowWindow(_selectedEntry, ChangeLocalizationKey);
        }

        private void ChangeLocalizationKey(LocalizationEntry selectedEntry)
        {
            _guidProperty.stringValue = selectedEntry.GUID;
            _guidProperty.serializedObject.ApplyModifiedProperties();

            UpdateLocalizationKey(selectedEntry);
        }

        private void SetupKeyField(string label)
        {
            _keyDropdown = new DropdownField
            {
                label = label,
                value = "[None]",
                style = { marginLeft = 0 }
            };

            _keyDropdown.AddUnityFileStyles();
            _keyDropdown.RegisterInputClick(this, static self => self.ShowKeySelectionWindow());

            if (_keyDropdown.TryQ<Label>(out var fieldLabel) is false)
                return;

            fieldLabel.style.marginLeft = 0;
            fieldLabel.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                AppendCopyAction(evt);
                AppendPasteAction(evt);
            }));
        }

        private void AppendCopyAction(ContextualMenuPopulateEvent menuPopulateEvent)
        {
            var copyState = _selectedEntry != null
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;

            menuPopulateEvent.menu.AppendAction("Copy Localization Key", CopyGUID, copyState);
        }

        private void AppendPasteAction(ContextualMenuPopulateEvent menuPopulateEvent)
        {
            var clipboardContent = EditorGUIUtility.systemCopyBuffer;
            var pasteState = LocalizationRegistry.Instance.Entries.ContainsKey(clipboardContent)
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;

            menuPopulateEvent.menu.AppendAction("Paste Localization Key", PasteLocalizationKey, pasteState);
        }

        private void CopyGUID(DropdownMenuAction _)
        {
            EditorGUIUtility.systemCopyBuffer = _selectedEntry?.GUID;
        }

        private void PasteLocalizationKey(DropdownMenuAction _)
        {
            if (string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
                ChangeLocalizationKey(null);

            var guid = EditorGUIUtility.systemCopyBuffer;
            if (LocalizationRegistry.Instance.Entries.TryGetValue(guid, out var entry))
                ChangeLocalizationKey(entry);
        }

        private void SetupToggle()
        {
            if (_translationsContainer.TryQ<Foldout>(out var foldout) is false)
                return;

            foldout.value = false;

            if (foldout.TryQ<Toggle>(out var toggle) is false)
                return;

            if (toggle.TryQ<Label>(out var toggleLabel))
                toggleLabel.RemoveFromHierarchy();

            if (toggle.TryQ<VisualElement>(out var toggleInput, className: Toggle.inputUssClassName))
                toggleInput.Add(_keyDropdown);
        }
    }
}