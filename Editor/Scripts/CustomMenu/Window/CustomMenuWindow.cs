using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.CustomMenu.Window
{
    internal sealed class CustomMenuWindow : WindowBase
    {
        private Vector2 _scrollPosition;

        protected override void InitializeWindow()
        {
            serializedObject = new SerializedObject(CustomMenuSettings.Instance);
        }

        [MenuItem(MenuItemNames.CustomMenuSettingsMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<CustomMenuWindow>(nameof(CustomMenuWindow).ToSpacedWords());
        }

        protected override void DrawWindowContent()
        {
            EditorVisualControls.H1Label("Custom Menu Settings");

            DrawPropertyFields();

            DrawGenerateMenuItems();

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(CustomMenuSettings.Instance);
        }

        private void DrawPropertyFields()
        {
            using var scrollScope = EditorVisualControls.CreateScrollView(ref _scrollPosition);

            EditorVisualControls.InfoBox(
                "Shortcut Format: _[modifiers][key]\n" +
                "Modifiers: % = Ctrl, & = Alt, # = Shift (can be combined)\n" +
                "Examples: _h (H key), _%h (Ctrl+H), _&h (Alt+H), _#h (Shift+H), _#&g (Shift+Alt+G), _%#s (Ctrl+Shift+S), _F5 (F5 key)\n" +
                "Valid keys: a-z, F1-F12");

            PropertyField(nameof(CustomMenuSettings.DefaultSceneAsset));
            PropertyField(nameof(CustomMenuSettings.SceneMenuItems));
            PropertyField(nameof(CustomMenuSettings.AssetMenuItems));
            PropertyField(nameof(CustomMenuSettings.PrefabMenuItems));
            PropertyField(nameof(CustomMenuSettings.MethodExecutionItems));
            PropertyField(nameof(CustomMenuSettings.ScriptingSymbols));
        }

        private void DrawGenerateMenuItems()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorVisualControls.Button("Generate Menu Items", GUILayout.Width(150), GUILayout.Height(30)))
            {
                serializedObject.ApplyModifiedProperties();

                MenuController.GenerateMenuItemsScriptFromSettings(CustomMenuSettings.Instance);

                EditorUtility.SetDirty(CustomMenuSettings.Instance);
                AssetDatabase.SaveAssets();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}