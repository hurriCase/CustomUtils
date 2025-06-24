using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Global
namespace CustomUtils.Editor.CustomMenu.MenuItems.Helpers
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static string EnableSetPlayModeSceneKey => $"{PlayerSettings.applicationIdentifier}.enableSetPlayModeScene";

        private static bool IsChangePlayModeScene
        {
            get => EditorPrefs.GetBool(EnableSetPlayModeSceneKey, false);
            set => EditorPrefs.SetBool(EnableSetPlayModeSceneKey, value);
        }

        static DefaultSceneLoader()
        {
            EditorApplication.delayCall += ChangePlayModeScene;
            EditorApplication.quitting += OnEditorQuitting;

            ChangePlayModeScene();
        }

        public static void ToggleAutoLoad()
        {
            IsChangePlayModeScene = IsChangePlayModeScene is false;

            Debug.Log($"Auto load startup scene is now {(IsChangePlayModeScene ? "enabled" : "disabled")}");

            ChangePlayModeScene();
        }

        public static bool IsDefaultSceneSet() => EditorPrefs.GetBool(EnableSetPlayModeSceneKey, false);

        private static void OnEditorQuitting()
        {
            EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
            EditorApplication.quitting -= OnEditorQuitting;
        }

        private static void ChangePlayModeScene()
        {
            if (IsChangePlayModeScene is false)
            {
                var currentScene = GetAsset<SceneAsset>(SceneManager.GetActiveScene().name);
                EditorSceneManager.playModeStartScene = currentScene;
                return;
            }

            var startUpScene = CustomMenuSettings.Instance.DefaultSceneAsset;

            if (startUpScene)
                EditorSceneManager.playModeStartScene = startUpScene;
        }

        private static T GetAsset<T>(string name) where T : Object
        {
            var assets = AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");
            if (assets.Length > 0)
                return (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T));

            return null;
        }

        private static void OnSceneChanged(Scene oldScene, Scene newScene) => ChangePlayModeScene();
    }
}