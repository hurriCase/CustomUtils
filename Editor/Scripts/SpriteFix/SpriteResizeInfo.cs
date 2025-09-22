using System;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.SpriteFix
{
    [Serializable]
    internal sealed class SpriteResizeInfo
    {
        internal string Path { get; set; }
        internal Sprite Sprite { get; set; }
        internal Vector2 OriginalSize { get; set; }
        internal Vector2 NewSize { get; set; }
        internal bool ShouldResize => OriginalSize != NewSize;
    }
}