using CustomUtils.Editor.EditorTheme;
using CustomUtils.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomMenu.Window
{
    internal sealed class CustomMenuWindow : WindowBase
    {
        private SerializedObject _serializedObject;
        private CustomMenuSettings Settings => CustomMenuSettings.Instance;

        private SerializedProperty _defaultSceneAssetProperty;
        private SerializedProperty _sceneMenuItemsProperty;
        private SerializedProperty _assetMenuItemsProperty;
        private SerializedProperty _methodExecutionItemsProperty;
        private SerializedProperty _scriptingSymbolsProperty;

        private Vector2 _scrollPosition;

        [MenuItem(MenuItemNames.CustomMenuSettingsMenuName)]
        internal static void ShowWindow()
        {
            var window = GetWindow<CustomMenuWindow>(nameof(CustomMenuWindow).ToSpacedWords());
            window.minSize = new Vector2(450, 600);
            window.Show();
        }

        protected override void InitializeWindow()
        {
            _serializedObject = new SerializedObject(Settings);

            _defaultSceneAssetProperty = _serializedObject.FindField(nameof(Settings.DefaultSceneAsset));
            _sceneMenuItemsProperty = _serializedObject.FindField(nameof(Settings.SceneMenuItems));
            _assetMenuItemsProperty = _serializedObject.FindField(nameof(Settings.AssetMenuItems));
            _methodExecutionItemsProperty = _serializedObject.FindField(nameof(Settings.MethodExecutionItems));
            _scriptingSymbolsProperty = _serializedObject.FindField(nameof(Settings.ScriptingSymbols));
        }

        protected override void DrawWindowContent()
        {
            EditorVisualControls.LabelField("Custom Menu Settings", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorStateControls.PropertyField(_defaultSceneAssetProperty, "DefaultScene");
            EditorStateControls.PropertyField(_sceneMenuItemsProperty, true);
            EditorStateControls.PropertyField(_assetMenuItemsProperty, true);
            EditorStateControls.PropertyField(_methodExecutionItemsProperty, true);
            EditorStateControls.PropertyField(_scriptingSymbolsProperty, true);

            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorVisualControls.Button("Generate Menu Items", GUILayout.Width(150), GUILayout.Height(30)))
            {
                _serializedObject.ApplyModifiedProperties();
                MenuManager.GenerateMenuItemsScriptFromSettings(Settings);
                EditorUtility.SetDirty(Settings);
                AssetDatabase.SaveAssets();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (_serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(Settings);
        }
    }
}