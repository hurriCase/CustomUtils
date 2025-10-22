using System;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationKeySelectorWindow : WindowBase
    {
        private Vector2 _scrollPosition;
        private string _searchText = string.Empty;
        private string _selectedTable = string.Empty;
        private SerializedProperty _targetProperty;
        private Action _onSelectionChanged;

        internal static void Show(SerializedProperty property, Action onSelectionChanged)
        {
            var window = GetWindow<LocalizationKeySelectorWindow>(true, "Select Localization Key", true);
            window._targetProperty = property;
            window._onSelectionChanged = onSelectionChanged;
            window.ShowUtility();
        }

        protected override void DrawWindowContent()
        {
            DrawTableFilter();
            DrawSearchField();

            DrawCurrentSelection();

            EditorVisualControls.DrawHorizontalLine();

            DrawEntryList();
        }

        private void DrawTableFilter()
        {
            var tables = LocalizationRegistry.Instance.GetAllTableNames();
            var tableOptions = new string[tables.Length + 1];
            tableOptions[0] = "All Tables";
            Array.Copy(tables, 0, tableOptions, 1, tables.Length);

            var currentIndex = string.IsNullOrEmpty(_selectedTable)
                ? 0
                : Array.IndexOf(tableOptions, _selectedTable);

            if (currentIndex == -1)
                currentIndex = 0;

            EditorGUILayout.BeginHorizontal();
            EditorVisualControls.LabelField("Table:", GUILayout.Width(50));
            var newIndex = EditorGUILayout.Popup(currentIndex, tableOptions);
            EditorGUILayout.EndHorizontal();

            _selectedTable = newIndex == 0 ? string.Empty : tableOptions[newIndex];
        }

        private void DrawSearchField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorVisualControls.LabelField("Search:", GUILayout.Width(50));

            GUI.SetNextControlName("SearchField");
            _searchText = EditorGUILayout.TextField(_searchText);

            if (EditorVisualControls.Button("Clear", GUILayout.Width(50)))
            {
                _searchText = string.Empty;
                GUI.FocusControl("SearchField");
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawCurrentSelection()
        {
            var guidProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
            var keyProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Key));
            var tableProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.TableName));

            EditorVisualControls.LabelField("Current Selection:", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(guidProperty.stringValue))
                EditorVisualControls.LabelField("None", EditorStyles.miniLabel);
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorVisualControls.LabelField($"Key: {keyProperty.stringValue}");
                EditorVisualControls.LabelField($"Table: {tableProperty.stringValue}");
                EditorVisualControls.LabelField($"GUID: {guidProperty.stringValue}", EditorStyles.miniLabel);

                if (GUILayout.Button("Clear Selection"))
                    ClearSelection();

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawEntryList()
        {
            var entries = GetFilteredEntries();

            EditorGUILayout.LabelField($"Available Keys ({entries.Length}):", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (entries.Length == 0)
                EditorVisualControls.LabelField("No localization keys found.", EditorStyles.centeredGreyMiniLabel);
            else
                for (var i = 0; i < entries.Length; i++)
                {
                    var backgroundColor = i % 2 == 0
                        ? new Color(0.3f, 0.3f, 0.3f, 0.2f)
                        : new Color(0.2f, 0.2f, 0.2f, 0.2f);

                    var rect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y, rect.width + 4, EditorGUIUtility.singleLineHeight), backgroundColor);

                    DrawEntryButton(entries[i]);

                    EditorGUILayout.EndHorizontal();
                }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEntryButton(LocalizationEntry entry)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(entry.Key, EditorStyles.boldLabel, GUILayout.Width(200));

            EditorGUILayout.LabelField($"Table: {entry.TableName}", EditorStyles.miniLabel, GUILayout.Width(120));

            var preview = GetPreviewText(entry);
            if (string.IsNullOrEmpty(preview) is false)
                EditorVisualControls.LabelField($"Preview: {preview}", EditorStyles.miniLabel);

            if (EditorVisualControls.Button("Select", GUILayout.Width(60)))
            {
                SelectEntry(entry);
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }

        private string GetPreviewText(LocalizationEntry entry)
        {
            var defaultLanguage = LocalizationDatabase.Instance.DefaultLanguage;

            if (entry.TryGetTranslation(defaultLanguage, out var text) &&
                string.IsNullOrEmpty(text) is false)
                return text.Length > 50 ? text.Substring(0, 47) + "..." : text;

            return "[No translation]";
        }

        private void SelectEntry(LocalizationEntry entry)
        {
            var guidProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
            var keyProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Key));
            var tableProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.TableName));

            guidProperty.stringValue = entry.Guid;
            keyProperty.stringValue = entry.Key;
            tableProperty.stringValue = entry.TableName;

            _targetProperty.serializedObject.ApplyModifiedProperties();
            _onSelectionChanged?.Invoke();
        }

        private void ClearSelection()
        {
            var guidProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Guid));
            var keyProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.Key));
            var tableProperty = _targetProperty.FindFieldRelative(nameof(LocalizationKey.TableName));

            guidProperty.stringValue = string.Empty;
            keyProperty.stringValue = string.Empty;
            tableProperty.stringValue = string.Empty;

            _targetProperty.serializedObject.ApplyModifiedProperties();
            _onSelectionChanged?.Invoke();
        }

        private LocalizationEntry[] GetFilteredEntries()
        {
            var tableName = string.IsNullOrEmpty(_selectedTable) ? null : _selectedTable;
            var entries = LocalizationRegistry.Instance.SearchEntries(_searchText, tableName);

            return entries.AsValueEnumerable()
                .OrderBy(static e => e.TableName)
                .ThenBy(static e => e.Key)
                .ToArray();
        }
    }
}