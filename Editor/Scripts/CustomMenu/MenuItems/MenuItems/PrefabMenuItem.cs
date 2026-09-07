using System;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.CustomMenu.MenuItems.MenuItems
{
    [Serializable]
    internal sealed class PrefabMenuItem : BaseMenuItem<GameObject>
    {
        [field: SerializeField] internal bool StretchToParent { get; private set; }
    }
}