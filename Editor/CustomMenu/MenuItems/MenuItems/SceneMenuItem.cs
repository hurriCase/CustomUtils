using System;
using UnityEditor;

namespace CustomUtils.Editor.CustomMenu.MenuItems.MenuItems
{
    [Serializable]
    internal sealed class SceneMenuItem : BaseMenuItem<SceneAsset>
    {
        internal string ScenePath => AssetDatabase.GetAssetPath(MenuTarget);
        internal string SceneName => MenuTarget.name;
    }
}