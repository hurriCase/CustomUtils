using System;
using static UnityEngine.RenderTexture;

namespace CustomUtils.Editor.SpriteFix
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class SpriteResizer : EditorWindow
    {
        private enum Mode
        {
            SingleSprite,
            Folder
        }

        private Mode _currentMode = Mode.SingleSprite;
        private Sprite _selectedSprite;
        private DefaultAsset _selectedFolder;
        private Vector2 _scrollPosition;
        private bool _showPreview = true;
        private readonly List<SpriteResizeInfo> _spritesToResize = new();
        private bool _processingComplete;

        private sealed class SpriteResizeInfo
        {
            public string path;
            public Sprite sprite;
            public Vector2 originalSize;
            public Vector2 newSize;
            public bool ShouldResize => originalSize != newSize;
        }

        [MenuItem("Tools/FixNPOT")]
        public static void ShowWindow()
        {
            GetWindow<SpriteResizer>("Sprite Resizer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Sprite Resizer", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            _currentMode = (Mode)EditorGUILayout.EnumPopup("Mode", _currentMode);
            EditorGUILayout.Space();

            switch (_currentMode)
            {
                case Mode.SingleSprite:
                    DrawSingleSpriteMode();
                    break;

                case Mode.Folder:
                    DrawFolderMode();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawSingleSpriteMode()
        {
            EditorGUI.BeginChangeCheck();
            _selectedSprite =
                (Sprite)EditorGUILayout.ObjectField("Select Sprite", _selectedSprite, typeof(Sprite), false);

            if (EditorGUI.EndChangeCheck() && _selectedSprite)
            {
                _spritesToResize.Clear();
                AddSpriteToList(_selectedSprite);
                _processingComplete = false;
            }

            if (_selectedSprite is null)
                return;

            DrawSpriteList();
            EditorGUILayout.Space();
            if (GUILayout.Button("Resize Sprite"))
                ProcessSprites(_spritesToResize);
        }

        private void DrawFolderMode()
        {
            EditorGUI.BeginChangeCheck();
            _selectedFolder =
                (DefaultAsset)EditorGUILayout.ObjectField("Select Folder", _selectedFolder, typeof(DefaultAsset),
                    false);

            if (EditorGUI.EndChangeCheck() && _selectedFolder)
            {
                var folderPath = AssetDatabase.GetAssetPath(_selectedFolder);
                if (Directory.Exists(folderPath))
                {
                    FindSpritesInFolder(folderPath);
                    _processingComplete = false;
                }
                else
                {
                    _selectedFolder = null;
                    EditorUtility.DisplayDialog("Error", "Please select a valid folder.", "OK");
                }
            }

            if (_selectedFolder is null)
                return;

            _showPreview = EditorGUILayout.Toggle("Show Preview", _showPreview);
            if (_showPreview)
                DrawSpriteList();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                string folderPath = AssetDatabase.GetAssetPath(_selectedFolder);
                FindSpritesInFolder(folderPath);
                _processingComplete = false;
            }

            if (GUILayout.Button("Resize All Sprites"))
                ProcessSprites(_spritesToResize);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpriteList()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sprites to process:", EditorStyles.boldLabel);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var spriteInfo in _spritesToResize)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Sprite: " + Path.GetFileName(spriteInfo.path));
                EditorGUILayout.LabelField("Original Size: " + spriteInfo.originalSize.x + " x " +
                                           spriteInfo.originalSize.y);
                EditorGUILayout.LabelField("New Size: " + spriteInfo.newSize.x + " x " + spriteInfo.newSize.y);
                if (spriteInfo.ShouldResize is false)
                    EditorGUILayout.LabelField("(No resize needed)", EditorStyles.miniLabel);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            if (_processingComplete)
                EditorGUILayout.HelpBox("Processing complete!", MessageType.Info);
        }

        private void FindSpritesInFolder(string folderPath)
        {
            _spritesToResize.Clear();
            var guids = AssetDatabase.FindAssets("t:sprite", new[] { folderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite)
                    AddSpriteToList(sprite);
            }
        }

        private void AddSpriteToList(Sprite sprite)
        {
            var originalSize = new Vector2(sprite.texture.width, sprite.texture.height);
            var newSize = new Vector2(
                RoundToNearestMultipleOf4((int)originalSize.x),
                RoundToNearestMultipleOf4((int)originalSize.y)
            );
            _spritesToResize.Add(new SpriteResizeInfo
            {
                path = AssetDatabase.GetAssetPath(sprite),
                sprite = sprite,
                originalSize = originalSize,
                newSize = newSize
            });
        }

        private int RoundToNearestMultipleOf4(int value)
            => Mathf.CeilToInt(value / 4f) * 4;

        private void ProcessSprites(List<SpriteResizeInfo> spriteInfos)
        {
            var processedCount = 0;
            var totalToProcess = spriteInfos.Count(s => s.ShouldResize);

            foreach (var spriteInfo in spriteInfos)
            {
                if (spriteInfo.ShouldResize is false)
                    continue;

                EditorUtility.DisplayProgressBar(
                    "Processing Sprites",
                    $"Processing {Path.GetFileName(spriteInfo.path)} ({processedCount + 1}/{totalToProcess})",
                    (float)processedCount / totalToProcess
                );
                ResizeSprite(spriteInfo);
                processedCount++;
            }

            EditorUtility.ClearProgressBar();
            _processingComplete = true;
            EditorUtility.DisplayDialog("Sprite Resizer",
                processedCount > 0
                    ? $"Successfully processed {processedCount} sprite{(processedCount != 1 ? "s" : "")}!"
                    : "No sprites needed resizing!", "OK");
        }

        private void ResizeSprite(SpriteResizeInfo spriteInfo)
        {
            var sourceTexture = spriteInfo.sprite.texture;

            var renderTexture = new RenderTexture((int)spriteInfo.newSize.x, (int)spriteInfo.newSize.y, 0);
            active = renderTexture;
            Graphics.Blit(sourceTexture, renderTexture);
            var newTexture = new Texture2D((int)spriteInfo.newSize.x, (int)spriteInfo.newSize.y);
            newTexture.ReadPixels(new Rect(0, 0, (int)spriteInfo.newSize.x, (int)spriteInfo.newSize.y), 0, 0);
            newTexture.Apply();
            active = null;
            renderTexture.Release();

            var bytes = newTexture.EncodeToPNG();
            File.WriteAllBytes(spriteInfo.path, bytes);
            AssetDatabase.ImportAsset(spriteInfo.path);

            if (AssetImporter.GetAtPath(spriteInfo.path) is not TextureImporter importer)
                return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
    }
}