using System;
using System.IO;
using CustomUtils.Editor.EditorTheme;
using CustomUtils.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Editor
{
    internal sealed class FlatScriptsCreatorWindow : WindowBase
    {
        private Object _sourceFolder;
        private Object _targetFolder;
        private Vector2 _scrollPosition;
        private bool _showSuccessMessage;
        private string _statusMessage = string.Empty;
        private float _messageTimer;

        [MenuItem(MenuItemNames.FlatScriptMenuName)]
        internal static void ShowWindow()
        {
            var window = GetWindow<FlatScriptsCreatorWindow>(nameof(FlatScriptsCreatorWindow).ToSpacedWords());
            window.minSize = new Vector2(400, 200);
        }

        protected override void InitializeWindow()
        {
            _statusMessage = string.Empty;
            _messageTimer = 0f;
        }

        protected override void CleanupWindow() { }

        protected override void DrawWindowContent()
        {
            DrawSection("Folder Selection", DrawFolderSelectionContent);
            DrawSection("Quick Folder Setup", DrawDragAndDropContent);

            if (GUILayout.Button("Create Flat Scripts", GUILayout.Height(30)))
                CreateFlatScripts();

            DrawStatusMessage();

            UpdateMessageTimer();
        }

        private void DrawFolderSelectionContent()
        {
            _sourceFolder = EditorStateControls.ObjectField("Source Folder:", _sourceFolder, typeof(DefaultAsset));
            _targetFolder = EditorStateControls.ObjectField("Target Folder:", _targetFolder, typeof(DefaultAsset));
        }

        private void DrawDragAndDropContent()
        {
            var dragDropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dragDropArea, "Drag and Drop Folders Here", EditorStyles.helpBox);
            HandleDragAndDrop(dragDropArea);
        }

        private void DrawStatusMessage()
        {
            if (string.IsNullOrEmpty(_statusMessage))
                return;

            if (_showSuccessMessage)
                EditorVisualControls.WarningBox(_statusMessage);
            else
                EditorVisualControls.ErrorBox(_statusMessage);
        }

        private void UpdateMessageTimer()
        {
            if (_messageTimer <= 0)
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
                        if (!Directory.Exists(path))
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