using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomMenu.Window
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

            PropertyField(nameof(CustomMenuSettings.DefaultSceneAsset));
            PropertyField(nameof(CustomMenuSettings.SceneMenuItems), true);
            PropertyField(nameof(CustomMenuSettings.AssetMenuItems), true);
            PropertyField(nameof(CustomMenuSettings.MethodExecutionItems), true);
            PropertyField(nameof(CustomMenuSettings.ScriptingSymbols), true);
        }

        private void DrawGenerateMenuItems()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorVisualControls.Button("Generate Menu Items", GUILayout.Width(150), GUILayout.Height(30)))
            {
                serializedObject.ApplyModifiedProperties();

                MenuManager.GenerateMenuItemsScriptFromSettings(CustomMenuSettings.Instance);

                EditorUtility.SetDirty(CustomMenuSettings.Instance);
                AssetDatabase.SaveAssets();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}