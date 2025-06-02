using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Editor
{
    /// <inheritdoc />
    /// <summary>
    /// Editor window utility for marking Unity objects as dirty to force saving their state.
    /// </summary>
    internal sealed class DirtyMakerWindow : WindowBase
    {
        private readonly List<Object> _objectsToProcess = new();

        private Vector2 _scrollPosition;
        private bool _autoSaveAssets = true;
        private bool _processCompleted;

        [MenuItem(MenuItemNames.DirtyMakerMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<DirtyMakerWindow>(nameof(DirtyMakerWindow).ToSpacedWords());
        }

        protected override void DrawWindowContent()
        {
            EditorVisualControls.InfoBox(
                "This tool marks any Unity objects as dirty, forcing Unity to save their current state.");

            DrawDropArea();

            _autoSaveAssets = EditorStateControls.Toggle("Auto-save assets after marking dirty", _autoSaveAssets);

            DrawObjectList();

            DrawActionButtons();

            if (_objectsToProcess.Count > 0 && _processCompleted)
                EditorVisualControls.InfoBox("Processing complete. Successfully marked " +
                                             $"{_objectsToProcess.Count} objects as dirty.");
        }

        private void DrawDropArea()
        {
            EditorVisualControls.H2Label("Drag and drop any objects here:");

            var wasDropped = EditorVisualControls.DrawObjectDropArea(
                100f,
                "Drop Objects Here",
                in _objectsToProcess);

            if (wasDropped)
                _processCompleted = false;
        }

        private void DrawObjectList()
        {
            if (_objectsToProcess.Count <= 0)
                return;

            using var sectionScope = BeginSection("Objects to Process");

            EditorVisualControls.LabelField($"Total: {_objectsToProcess.Count} objects");

            using var scrollScope = EditorVisualControls.CreateScrollView(ref _scrollPosition);

            var itemsToRemove = new List<int>();

            for (var i = 0; i < _objectsToProcess.Count; i++)
            {
                using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

                if (!_objectsToProcess[i])
                {
                    itemsToRemove.Add(i);
                    EditorVisualControls.LabelField("Missing Object");
                }
                else
                    _objectsToProcess[i] = EditorStateControls.ObjectField(_objectsToProcess[i], typeof(Object));

                if (EditorVisualControls.Button("Remove"))
                    itemsToRemove.Add(i);
            }

            for (var i = itemsToRemove.Count - 1; i >= 0; i--)
                _objectsToProcess.RemoveAt(itemsToRemove[i]);
        }

        private void DrawActionButtons()
        {
            if (_objectsToProcess.Count <= 0)
                return;

            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            EditorGUI.BeginDisabledGroup(_objectsToProcess.Count == 0);

            if (EditorVisualControls.Button("Mark All Objects as Dirty", GUILayout.Height(30)))
                MarkObjectsAsDirty();

            EditorGUI.EndDisabledGroup();

            if (EditorVisualControls.Button("Clear All", GUILayout.Height(30)))
                _objectsToProcess.Clear();
        }

        private void MarkObjectsAsDirty()
        {
            Undo.RecordObjects(_objectsToProcess.ToArray(), "Mark Objects as Dirty");

            foreach (var objectToProcess in _objectsToProcess)
            {
                if (!objectToProcess)
                    continue;

                EditorUtility.SetDirty(objectToProcess);
            }

            _processCompleted = true;

            _objectsToProcess.Clear();

            if (_autoSaveAssets)
                AssetDatabase.SaveAssets();
        }
    }
}