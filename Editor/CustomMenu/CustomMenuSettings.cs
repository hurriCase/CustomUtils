using CustomUtils.Editor.CustomMenu.MenuItems.MenuItems;
using CustomUtils.Editor.CustomMenu.MenuItems.MenuItems.MethodExecution;
using CustomUtils.Runtime;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomMenu
{
    [Resource(
        ResourcePaths.CustomMenuResourcePath,
        ResourcePaths.CustomMenuSettingsAssetName,
        isEditorResource: true
    )]
    internal sealed class CustomMenuSettings : SingletonScriptableObject<CustomMenuSettings>
    {
        [field: SerializeField] internal SceneAsset DefaultSceneAsset { get; private set; }
        [field: SerializeField] internal SceneMenuItem[] SceneMenuItems { get; private set; }
        [field: SerializeField] internal AssetMenuItem[] AssetMenuItems { get; private set; }
        [field: SerializeField] internal MethodExecutionMenuItem[] MethodExecutionItems { get; private set; }
        [field: SerializeField] internal ScriptingSymbolMenuItem[] ScriptingSymbols { get; private set; }
    }
}