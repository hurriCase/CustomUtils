using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace CustomUtils.Editor.SpriteFix
{
    public sealed class SpriteAlphaAdder : EditorWindow
    {
        private Sprite _targetSprite;
        private Vector2 _scrollPosition;
        private readonly List<TextureImporter> _problematicSprites = new();
        private bool _showProblematicSprites;

        [MenuItem("Tools/Add Alpha To Sprite")]
        public static void ShowWindow()
        {
            GetWindow<SpriteAlphaAdder>("Alpha Adder");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Add Alpha Pixel To Sprite", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            DrawScan();
            DrawManualSelection();
        }

        private void DrawScan()
        {
            if (GUILayout.Button("Scan Project for Problematic Sprites"))
                FindProblematicSprites();

            if (_problematicSprites.Count <= 0)
                return;

            EditorGUILayout.Space(10);
            _showProblematicSprites = EditorGUILayout.Foldout(_showProblematicSprites,
                $"Problematic Sprites Found: {_problematicSprites.Count}", true);

            if (_showProblematicSprites is false)
                return;

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            foreach (var importer in _problematicSprites)
            {
                EditorGUILayout.BeginHorizontal();

                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
                if (texture)
                    GUILayout.Label(AssetPreview.GetAssetPreview(texture), GUILayout.Width(50),
                        GUILayout.Height(50));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(Path.GetFileName(importer.assetPath));
                EditorGUILayout.LabelField($"Size: {texture?.width}x{texture?.height}", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = texture;
                    EditorGUIUtility.PingObject(texture);
                }

                if (GUILayout.Button("Fix", GUILayout.Width(40)))
                    AddAlphaPixel(importer);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Fix All"))
                FixAllSprites();
        }

        private void DrawManualSelection()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Manual Selection", EditorStyles.boldLabel);
            _targetSprite = (Sprite)EditorGUILayout.ObjectField("Target Sprite", _targetSprite, typeof(Sprite), false);
            if (_targetSprite is null)
                return;

            var path = AssetDatabase.GetAssetPath(_targetSprite);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            var isRGBA = importer && importer.DoesSourceTextureHaveAlpha();
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Current Format: {(isRGBA ? "RGBA" : "RGB")}", EditorStyles.boldLabel);
            if (isRGBA is false)
            {
                EditorGUILayout.Space(10);
                if (GUILayout.Button("Add Alpha Pixel"))
                    AddAlphaPixel(importer);
            }
            else
                EditorGUILayout.HelpBox("This sprite already has an alpha channel.", MessageType.Info);
        }

        private void FindProblematicSprites()
        {
            _problematicSprites.Clear();

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
                if (texture is null)
                    continue;

                var isNPOT = IsPowerOfTwo(texture.width) is false && IsPowerOfTwo(texture.height) is false;
                var isRGB8 = !importer.DoesSourceTextureHaveAlpha();
                var hasCrunchEnabled = importer.crunchedCompression;

                if (isNPOT is false || isRGB8 is false || hasCrunchEnabled is false)
                    continue;

                _problematicSprites.Add(importer);
                Debug.Log($"Found problematic sprite: {path} ({texture.width}x{texture.height})");
            }

            Debug.Log($"Found {_problematicSprites.Count} problematic sprites");
        }

        private bool IsPowerOfTwo(int x) => x != 0 && (x & (x - 1)) == 0;

        private void FixAllSprites()
        {
            foreach (var importer in _problematicSprites.ToList())
                AddAlphaPixel(importer);

            _problematicSprites.Clear();
        }

        private void AddAlphaPixel(TextureImporter textureImporter)
        {
            if (textureImporter is null)
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
                Debug.LogError($"Error adding alpha pixel to {path}: {e.Message}");

                textureImporter.SetTextureSettings(originalSettings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}