using System.Collections.Generic;
using CustomUtils.Editor.EditorTheme;
using CustomUtils.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Editor
{
    /// <summary>
    /// Editor window utility for marking Unity objects as dirty to force saving their state.
    /// </summary>
    internal sealed class DirtyMakerWindow : WindowBase
    {
        private readonly List<Object> _objectsToProcess = new();
        private readonly List<string> _errorMessages = new();

        private Vector2 _scrollPosition;
        private bool _processingComplete;
        private int _successCount;
        private bool _autoSaveAssets = true;

        [MenuItem(MenuItemNames.DirtyMakerMenuName)]
        internal static void ShowWindow()
        {
            var window = GetWindow<DirtyMakerWindow>(nameof(DirtyMakerWindow).ToSpacedWords());
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        protected override void InitializeWindow()
        {
            _processingComplete = false;
            _successCount = 0;
            _errorMessages.Clear();
        }

        protected override void DrawWindowContent()
        {
            EditorVisualControls.H1Label("Dirty Maker");
            EditorVisualControls.InfoBox(
                "This tool marks any Unity objects as dirty, forcing Unity to save their current state.");
            EditorGUILayout.Space();

            DrawDropArea();
            DrawOptionsArea();
            DrawObjectList();
            DrawActionButtons();
            DrawResults();
        }

        private void DrawDropArea()
        {
            EditorVisualControls.H2Label("Drag and drop any objects here:");

            var dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drop Objects Here");

            var evt = Event.current;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropArea.Contains(evt.mousePosition) is false)
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject && _objectsToProcess.Contains(draggedObject) is false)
                                _objectsToProcess.Add(draggedObject);
                        }

                        _processingComplete = false;
                        _successCount = 0;
                        _errorMessages.Clear();
                    }

                    break;
            }
        }

        private void DrawOptionsArea()
        {
            EditorGUILayout.Space();
            _autoSaveAssets = EditorStateControls.Toggle("Auto-save assets after marking dirty", _autoSaveAssets);
            EditorGUILayout.Space();
        }

        private void DrawObjectList()
        {
            if (_objectsToProcess.Count <= 0)
                return;

            DrawSection("Objects to Process", () =>
            {
                EditorVisualControls.LabelField($"Total: {_objectsToProcess.Count} objects");
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(150));

                var itemsToRemove = new List<int>();

                for (var i = 0; i < _objectsToProcess.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    if (!_objectsToProcess[i])
                    {
                        itemsToRemove.Add(i);
                        EditorGUILayout.LabelField("Missing Object");
                    }
                    else
                    {
                        _objectsToProcess[i] = EditorGUILayout.ObjectField(
                            _objectsToProcess[i].name, _objectsToProcess[i], typeof(Object), false);

                        EditorGUILayout.LabelField(_objectsToProcess[i].GetType().Name, GUILayout.Width(100));
                    }

                    if (EditorVisualControls.Button("Remove", GUILayout.Width(80)))
                        itemsToRemove.Add(i);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();

                // Remove items that were flagged for deletion
                for (var i = itemsToRemove.Count - 1; i >= 0; i--)
                    _objectsToProcess.RemoveAt(itemsToRemove[i]);
            });
        }

        private void DrawActionButtons()
        {
            if (_objectsToProcess.Count <= 0)
                return;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(_objectsToProcess.Count == 0);
            if (EditorVisualControls.Button("Mark All Objects as Dirty", GUILayout.Height(30)))
                MarkObjectsAsDirty();
            EditorGUI.EndDisabledGroup();

            if (EditorVisualControls.Button("Clear All", GUILayout.Height(30)))
            {
                _objectsToProcess.Clear();
                _processingComplete = false;
                _successCount = 0;
                _errorMessages.Clear();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawResults()
        {
            if (_processingComplete is false)
                return;

            EditorGUILayout.Space();

            var messageType = _successCount == _objectsToProcess.Count
                ? MessageType.Info
                : MessageType.Warning;

            EditorVisualControls.InfoBox($"Processing complete. Successfully marked {_successCount} objects as dirty.");

            if (_errorMessages.Count <= 0)
                return;

            DrawFoldoutSection("Errors", () =>
            {
                foreach (var error in _errorMessages)
                    EditorVisualControls.ErrorBox(error);
            });
        }

        private void MarkObjectsAsDirty()
        {
            _successCount = 0;
            _errorMessages.Clear();

            Undo.RecordObjects(_objectsToProcess.ToArray(), "Mark Objects as Dirty");

            foreach (var objectToProcess in _objectsToProcess)
            {
                if (!objectToProcess)
                    continue;

                try
                {
                    EditorUtility.SetDirty(objectToProcess);
                    _successCount++;
                }
                catch (System.Exception ex)
                {
                    _errorMessages.Add($"Error processing {objectToProcess.name}: {ex.Message}");
                }
            }

            if (_autoSaveAssets)
                AssetDatabase.SaveAssets();

            _processingComplete = true;
        }
    }
}