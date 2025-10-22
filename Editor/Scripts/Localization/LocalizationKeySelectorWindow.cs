using System;
using System.Linq;
using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.Localization
{
    internal sealed class LocalizationKeySelectorWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string _searchText = string.Empty;
        private string _selectedTable = string.Empty;
        private SerializedProperty _targetProperty;
        private Action _onSelectionChanged;

        private const float WindowWidth = 500f;
        private const float WindowHeight = 600f;
        private const float SearchFieldHeight = 20f;
        private const float TableDropdownHeight = 20f;
        private const float Spacing = 5f;

        internal static void Show(SerializedProperty property, Action onSelectionChanged)
        {
            var window = GetWindow<LocalizationKeySelectorWindow>(true, "Select Localization Key", true);
            window.minSize = new Vector2(WindowWidth, WindowHeight);
            window.maxSize = new Vector2(WindowWidth * 2, WindowHeight * 2);
            window._targetProperty = property;
            window._onSelectionChanged = onSelectionChanged;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            if (_targetProperty == null || _targetProperty.serializedObject.targetObject == null)
            {
                Close();
                return;
            }

            DrawHeader();
            DrawTableFilter();
            DrawSearchField();
            DrawCurrentSelection();
            DrawSeparator();
            DrawEntryList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(Spacing);
            EditorGUILayout.LabelField("Select Localization Key", EditorStyles.boldLabel);
            EditorGUILayout.Space(Spacing);
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
            EditorGUILayout.LabelField("Table:", GUILayout.Width(50));
            var newIndex = EditorGUILayout.Popup(currentIndex, tableOptions);
            EditorGUILayout.EndHorizontal();

            _selectedTable = newIndex == 0 ? string.Empty : tableOptions[newIndex];

            EditorGUILayout.Space(Spacing);
        }

        private void DrawSearchField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));

            GUI.SetNextControlName("SearchField");
            _searchText = EditorGUILayout.TextField(_searchText);

            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                _searchText = string.Empty;
                GUI.FocusControl("SearchField");
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(Spacing);
        }

        private void DrawCurrentSelection()
        {
            var guidProperty = _targetProperty.FindPropertyRelative("_guid");
            var keyProperty = _targetProperty.FindPropertyRelative("_key");
            var tableProperty = _targetProperty.FindPropertyRelative("_tableName");

            EditorGUILayout.LabelField("Current Selection:", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(guidProperty.stringValue))
            {
                EditorGUILayout.LabelField("None", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"Key: {keyProperty.stringValue}");
                EditorGUILayout.LabelField($"Table: {tableProperty.stringValue}");
                EditorGUILayout.LabelField($"GUID: {guidProperty.stringValue}", EditorStyles.miniLabel);

                if (GUILayout.Button("Clear Selection"))
                    ClearSelection();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(Spacing);
        }

        private void DrawSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(Spacing);
        }

        private void DrawEntryList()
        {
            var entries = GetFilteredEntries();

            EditorGUILayout.LabelField($"Available Keys ({entries.Length}):", EditorStyles.boldLabel);
            EditorGUILayout.Space(Spacing);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            if (entries.Length == 0)
            {
                EditorGUILayout.LabelField("No localization keys found.", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (var i = 0; i < entries.Length; i++)
                {
                    // Alternating row colors
                    var backgroundColor = i % 2 == 0
                        ? new Color(0.3f, 0.3f, 0.3f, 0.2f)
                        : new Color(0.2f, 0.2f, 0.2f, 0.2f);

                    var rect = EditorGUILayout.BeginHorizontal();
                    EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y, rect.width + 4, EditorGUIUtility.singleLineHeight), backgroundColor);

                    DrawEntryButton(entries[i]);

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEntryButton(LocalizationEntry entry)
        {
            EditorGUILayout.BeginHorizontal();

            // Key name (bold)
            EditorGUILayout.LabelField(entry.Key, EditorStyles.boldLabel, GUILayout.Width(200));

            // Table name
            EditorGUILayout.LabelField($"Table: {entry.TableName}", EditorStyles.miniLabel, GUILayout.Width(120));

            // Preview text
            var preview = GetPreviewText(entry);
            if (string.IsNullOrEmpty(preview) is false)
                EditorGUILayout.LabelField($"Preview: {preview}", EditorStyles.miniLabel);

            // Select button
            if (GUILayout.Button("Select", GUILayout.Width(60)))
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
            {
                return text.Length > 50 ? text.Substring(0, 47) + "..." : text;
            }

            return "[No translation]";
        }

        private void SelectEntry(LocalizationEntry entry)
        {
            var guidProperty = _targetProperty.FindPropertyRelative("_guid");
            var keyProperty = _targetProperty.FindPropertyRelative("_key");
            var tableProperty = _targetProperty.FindPropertyRelative("_tableName");

            guidProperty.stringValue = entry.Guid;
            keyProperty.stringValue = entry.Key;
            tableProperty.stringValue = entry.TableName;

            _targetProperty.serializedObject.ApplyModifiedProperties();
            _onSelectionChanged?.Invoke();
        }

        private void ClearSelection()
        {
            var guidProperty = _targetProperty.FindPropertyRelative("_guid");
            var keyProperty = _targetProperty.FindPropertyRelative("_key");
            var tableProperty = _targetProperty.FindPropertyRelative("_tableName");

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
                .OrderBy(e => e.TableName)
                .ThenBy(e => e.Key)
                .ToArray();
        }
    }
}