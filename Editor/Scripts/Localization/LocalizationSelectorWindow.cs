using System;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationSelectorWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _windowLayout;

        private const string AllTablesName = "all tables";

        private DropdownField _tableNamesDropdown;
        private TextField _searchTextField;
        private ListView _localizationEntriesList;

        private LocalizationEntry _currentLocalizationEntry;
        private Action<LocalizationEntry> _onSelectionChanged;

        internal static void ShowWindow(LocalizationEntry localizationEntry, Action<LocalizationEntry> onSelectionChanged)
        {
            var window = CreateInstance<LocalizationSelectorWindow>();

            window.titleContent = new GUIContent("Localization Selector");
            window._currentLocalizationEntry = localizationEntry;
            window._onSelectionChanged = onSelectionChanged;

            window.ShowUtility();
        }

        private void CreateGUI()
        {
            _windowLayout.CloneTree(rootVisualElement);

            SetupTableNamesDropdown();

            _searchTextField = rootVisualElement.Q<TextField>("LocalizationKeySearch");
            _searchTextField.RegisterValueChangedCallback(OnSearchChanged);

            SetupCurrentSelection();
            SetupLocalizationEntriesList();
        }

        private void SetupTableNamesDropdown()
        {
            _tableNamesDropdown = rootVisualElement.Q<DropdownField>("TableSelection");
            var tableNames = LocalizationRegistry.Instance.GetAllTableNames();
            tableNames.Insert(0, AllTablesName);
            _tableNamesDropdown.choices = tableNames;
            _tableNamesDropdown.value = tableNames[0];
            _tableNamesDropdown.RegisterValueChangedCallback(OnSearchChanged);
        }

        private void SetupCurrentSelection()
        {
            var noneSelectedLabel = rootVisualElement.Q<Label>("NoneSelectedLabel");
            var currentInfoContainer = rootVisualElement.Q<VisualElement>("CurrentSelectionInfo");

            if (_currentLocalizationEntry is null)
            {
                noneSelectedLabel.SetActive(true);
                currentInfoContainer.SetActive(false);
                return;
            }

            noneSelectedLabel.SetActive(_currentLocalizationEntry.IsValid is false);
            currentInfoContainer.SetActive(_currentLocalizationEntry.IsValid);

            if (_currentLocalizationEntry.IsValid is false)
                return;

            rootVisualElement.Q<Label>("KeyLabel").text = $"Key: {_currentLocalizationEntry.Key}";
            rootVisualElement.Q<Label>("TableNameLabel").text = $"Table: {_currentLocalizationEntry.TableName}";
            rootVisualElement.Q<Label>("GUIDLabel").text = $"GUID: {_currentLocalizationEntry.GUID}";
        }

        private void SetupLocalizationEntriesList()
        {
            _localizationEntriesList = rootVisualElement.Q<ListView>("EntriesList");
            _localizationEntriesList.itemsSource = LocalizationRegistry.Instance.SearchEntries(string.Empty);
            _localizationEntriesList.bindItem = BindItem;
        }

        private void OnSearchChanged(ChangeEvent<string> changeEvent)
        {
            var tableName = _tableNamesDropdown.value == AllTablesName ? null : _tableNamesDropdown.value;
            _localizationEntriesList.itemsSource = LocalizationRegistry.Instance.SearchEntries(
                _searchTextField.value,
                tableName);
        }

        private void BindItem(VisualElement element, int index)
        {
            var entry = (LocalizationEntry)_localizationEntriesList.itemsSource[index];

            element.Q<Label>("KeyLabel").text = entry.Key;
            element.Q<Label>("TableNameLabel").text = entry.TableName;

            var englishTranslation = entry.Translations[SystemLanguage.English];
            element.Q<Label>("LocalizationLabel").text = englishTranslation;
            element.Q<Button>("SelectButton").clicked += () => _onSelectionChanged?.Invoke(entry);
        }
    }
}