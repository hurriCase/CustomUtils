using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    public sealed class FlatScriptsCreatorWindow : EditorWindow
    {
        private UnityEngine.Object _sourceFolder;
        private UnityEngine.Object _targetFolder;
        private Vector2 _scrollPosition;
        private bool _showSuccessMessage;
        private string _statusMessage = string.Empty;
        private float _messageTimer;

        [MenuItem("Tools/Flat Scripts Creator")]
        public static void ShowWindow()
        {
            var window = GetWindow<FlatScriptsCreatorWindow>("Flat Scripts Creator");
            window.minSize = new Vector2(400, 200);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Flat Scripts Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Source Folder:", GUILayout.Width(100));
            _sourceFolder =
                EditorGUILayout.ObjectField(_sourceFolder, typeof(DefaultAsset), false, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Folder:", GUILayout.Width(100));
            _targetFolder =
                EditorGUILayout.ObjectField(_targetFolder, typeof(DefaultAsset), false, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            var dragDropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dragDropArea, "Drag and Drop Folders Here", EditorStyles.helpBox);

            HandleDragAndDrop(dragDropArea);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Create Flat Scripts", GUILayout.Height(30)))
                CreateFlatScripts();

            if (string.IsNullOrEmpty(_statusMessage) is false)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox(_statusMessage, _showSuccessMessage ? MessageType.Info : MessageType.Error);
            }

            if ((_messageTimer > 0) is false)
                return;

            _messageTimer -= Time.deltaTime;

            if ((_messageTimer <= 0) is false)
                return;

            _statusMessage = string.Empty;
            Repaint();
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            var currentEvent = Event.current;
            if (dropArea.Contains(currentEvent.mousePosition) is false)
                return;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    currentEvent.Use();
                    break;

                case EventType.DragPerform:
                    DragAndDrop.AcceptDrag();

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is not DefaultAsset)
                            continue;

                        var path = AssetDatabase.GetAssetPath(draggedObject);
                        if (Directory.Exists(path) is false)
                            continue;

                        if (_sourceFolder is null)
                            _sourceFolder = draggedObject;

                        else if (_targetFolder is null)
                            _targetFolder = draggedObject;
                    }

                    currentEvent.Use();
                    break;
            }
        }

        private void CreateFlatScripts()
        {
            if (_sourceFolder is null || _targetFolder is null)
            {
                ShowMessage("Please select both source and target folders.", false);
                return;
            }

            var sourcePath = AssetDatabase.GetAssetPath(_sourceFolder);
            var targetPath = AssetDatabase.GetAssetPath(_targetFolder);

            if (Directory.Exists(sourcePath) is false || Directory.Exists(targetPath) is false)
            {
                ShowMessage("Invalid folder path(s).", false);
                return;
            }

            try
            {
                var csFiles = Directory.GetFiles(sourcePath, "*.cs", SearchOption.AllDirectories);
                var copiedFiles = 0;

                foreach (var file in csFiles)
                {
                    var fileName = Path.GetFileName(file);
                    var targetFilePath = Path.Combine(targetPath, fileName);

                    if (File.Exists(targetFilePath))
                    {
                        var uniqueName =
                            $"{Path.GetFileNameWithoutExtension(file)}_{Guid.NewGuid()}{Path.GetExtension(file)}";
                        targetFilePath = Path.Combine(targetPath, uniqueName);
                    }

                    File.Copy(file, targetFilePath);
                    copiedFiles++;
                }

                AssetDatabase.Refresh();
                ShowMessage($"Successfully copied {copiedFiles} scripts to flat folder structure.", true);
            }
            catch (Exception e)
            {
                ShowMessage($"Error: {e.Message}", false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            _statusMessage = message;
            _showSuccessMessage = isSuccess;
            _messageTimer = 3f;
            Repaint();
        }
    }
}