using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CustomUtils.Editor.EditorTheme;
using CustomUtils.Editor.Extensions;

namespace CustomUtils.Editor.SpriteFix
{
    internal sealed class SpriteAlphaAdderWindow : WindowBase
    {
        private Sprite _targetSprite;
        private Vector2 _scrollPosition;
        private readonly List<TextureImporter> _problematicSprites = new();
        private bool _showProblematicSprites;

        [MenuItem(MenuItemNames.SpriteAlphaAdderMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<SpriteAlphaAdderWindow>(nameof(SpriteAlphaAdderWindow).ToSpacedWords());
        }

        protected override void InitializeWindow()
        {
            _showProblematicSprites = false;
        }

        protected override void DrawWindowContent()
        {
            DrawSection("Project Scanner", DrawScan);
            DrawSection("Manual Sprite Processing", DrawManualSelection);
        }

        private void DrawScan()
        {
            if (EditorVisualControls.Button("Scan Project for Problematic Sprites"))
                FindProblematicSprites();

            if (_problematicSprites.Count <= 0)
                return;

            _showProblematicSprites = EditorVisualControls
                .Foldout(_showProblematicSprites, $"Problematic Sprites Found: {_problematicSprites.Count}");

            if (_showProblematicSprites is false)
                return;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorVisualControls.DrawPanel(() =>
            {
                foreach (var importer in _problematicSprites)
                {
                    DrawSpriteEntry(importer);
                }
            });

            EditorGUILayout.EndScrollView();

            if (EditorVisualControls.Button("Fix All", GUILayout.Height(25)))
                FixAllSprites();
        }

        private void DrawSpriteEntry(TextureImporter importer)
        {
            EditorGUILayout.BeginHorizontal();

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
            if (texture)
                GUILayout.Label(AssetPreview.GetAssetPreview(texture),
                    GUILayout.Width(50), GUILayout.Height(50));

            EditorVisualControls.LabelField(Path.GetFileName(importer.assetPath));
            EditorVisualControls.LabelField($"Size: {texture?.width}x{texture?.height}", EditorStyles.miniLabel);

            if (EditorVisualControls.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeObject = texture;
                EditorGUIUtility.PingObject(texture);
            }

            if (EditorVisualControls.Button("Fix", GUILayout.Width(40)))
                AddAlphaPixel(importer);
        }

        private void DrawManualSelection()
        {
            _targetSprite = EditorStateControls.SpriteField("Target Sprite", _targetSprite);

            if (!_targetSprite)
                return;

            var path = AssetDatabase.GetAssetPath(_targetSprite);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            var isRGBA = importer && importer.DoesSourceTextureHaveAlpha();

            EditorVisualControls.H3Label($"Current Format: {(isRGBA ? "RGBA" : "RGB")}");

            if (isRGBA is false)
                EditorVisualControls.DrawPanel(() =>
                {
                    if (EditorVisualControls.Button("Add Alpha Pixel", GUILayout.Height(25)))
                        AddAlphaPixel(importer);
                });
            else
                EditorVisualControls.WarningBox("This sprite already has an alpha channel.");
        }

        private void FindProblematicSprites()
        {
            _problematicSprites.Clear();

            EditorVisualControls.DrawBoxedSection("Scan Progress",
                () => { EditorVisualControls.LabelField("Scanning project textures..."); });

            var guids = AssetDatabase.FindAssets("t:texture2d");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetImporter.GetAtPath(path) is not TextureImporter
                    {
                        textureType: TextureImporterType.Sprite
                    } importer)
                    continue;

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (!texture)
                    continue;

                var isNPOT = IsPowerOfTwo(texture.width) is false && IsPowerOfTwo(texture.height) is false;
                var isRGB8 = importer.DoesSourceTextureHaveAlpha() is false;
                var hasCrunchEnabled = importer.crunchedCompression;

                if (isNPOT is false || isRGB8 is false || hasCrunchEnabled is false)
                    continue;

                _problematicSprites.Add(importer);
                Debug.Log($"Found problematic sprite: {path} ({texture.width}x{texture.height})");
            }

            Debug.Log($"Found {_problematicSprites.Count} problematic sprites");

            if (_problematicSprites.Count > 0)
                EditorVisualControls.InfoBox(
                    $"Found {_problematicSprites.Count} problematic sprites that need alpha channel fixes.");
            else
                EditorGUILayout.HelpBox("No problematic sprites found. All sprites have proper alpha channels.",
                    MessageType.Info);
        }

        private bool IsPowerOfTwo(int x) => x != 0 && (x & (x - 1)) == 0;

        private void FixAllSprites()
        {
            var showProgress = false;
            EditorVisualControls.DrawBoxWithFoldout("Processing Progress", ref showProgress, () =>
            {
                foreach (var importer in _problematicSprites)
                {
                    EditorVisualControls.LabelField($"Processing: {Path.GetFileName(importer.assetPath)}");
                    AddAlphaPixel(importer);
                }
            });

            _problematicSprites.Clear();
        }

        private void AddAlphaPixel(TextureImporter textureImporter)
        {
            if (!textureImporter)
                return;

            var path = textureImporter.assetPath;

            var originalSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(originalSettings);
            try
            {
                textureImporter.isReadable = true;
                textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                textureImporter.alphaIsTransparency = true;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

                var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                var pixels = texture.GetPixels();

                var lastPixel = (texture.width * texture.height) - 1;
                pixels[lastPixel] = new Color(
                    pixels[lastPixel].r,
                    pixels[lastPixel].g,
                    pixels[lastPixel].b,
                    0.99f
                );
                newTexture.SetPixels(pixels);
                newTexture.Apply();

                var pngData = newTexture.EncodeToPNG();
                File.WriteAllBytes(path, pngData);

                DestroyImmediate(newTexture);

                textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                textureImporter.alphaIsTransparency = true;
                textureImporter.isReadable = originalSettings.readable;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.sRGBTexture = true;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Alpha pixel added successfully to {path}");
            }
            catch (Exception e)
            {
                EditorVisualControls.ErrorBox($"Error processing {Path.GetFileName(path)}: {e.Message}");
                Debug.LogError($"Error adding alpha pixel to {path}: {e.Message}");

                textureImporter.SetTextureSettings(originalSettings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}