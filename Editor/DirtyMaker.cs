using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.CustomUtils.Editor
{
    internal sealed class DirtyMaker : EditorWindow
    {
        private readonly List<Object> _objectsToProcess = new();
        private readonly List<string> _errorMessages = new();

        private Vector2 _scrollPosition;
        private bool _processingComplete;
        private int _successCount;
        private bool _autoSaveAssets = true;

        [MenuItem("Tools/Dirty Maker")]
        internal static void ShowWindow()
        {
            GetWindow<DirtyMaker>("Dirty Maker");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dirty Maker", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This tool marks any Unity objects as dirty, forcing Unity to save their current state.",
                MessageType.Info);
            EditorGUILayout.Space();

            GUILayout.Label("Drag and drop any objects here:", EditorStyles.boldLabel);

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

            EditorGUILayout.Space();

            _autoSaveAssets = EditorGUILayout.ToggleLeft("Auto-save assets after marking dirty", _autoSaveAssets);

            EditorGUILayout.Space();

            if (_objectsToProcess.Count > 0)
            {
                GUILayout.Label($"Objects to process ({_objectsToProcess.Count}):", EditorStyles.boldLabel);
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

                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                        itemsToRemove.Add(i);

                    EditorGUILayout.EndHorizontal();
                }

                for (var i = itemsToRemove.Count - 1; i >= 0; i--)
                    _objectsToProcess.RemoveAt(itemsToRemove[i]);

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(_objectsToProcess.Count == 0);

                if (GUILayout.Button("Mark All Objects as Dirty", GUILayout.Height(30)))
                    MarkObjectsAsDirty();

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Clear All", GUILayout.Height(25)))
                {
                    _objectsToProcess.Clear();
                    _processingComplete = false;
                    _successCount = 0;
                    _errorMessages.Clear();
                }
            }

            if (_processingComplete is false)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"Processing complete. Successfully marked {_successCount} objects as dirty.",
                _successCount == _objectsToProcess.Count ? MessageType.Info : MessageType.Warning);

            if (_errorMessages.Count <= 0)
                return;

            EditorGUILayout.LabelField("Errors:", EditorStyles.boldLabel);
            foreach (var error in _errorMessages)
                EditorGUILayout.HelpBox(error, MessageType.Error);
        }

        private void MarkObjectsAsDirty()
        {
            _successCount = 0;
            _errorMessages.Clear();

            Undo.RecordObjects(_objectsToProcess.ToArray(), "Mark Objects as Dirty");

            foreach (var objectToPrecess in _objectsToProcess)
            {
                if (!objectToPrecess)
                    continue;

                try
                {
                    EditorUtility.SetDirty(objectToPrecess);
                    _successCount++;
                }
                catch (System.Exception ex)
                {
                    _errorMessages.Add($"Error processing {objectToPrecess.name}: {ex.Message}");
                }
            }

            if (_autoSaveAssets)
                AssetDatabase.SaveAssets();

            _processingComplete = true;
        }
    }
}