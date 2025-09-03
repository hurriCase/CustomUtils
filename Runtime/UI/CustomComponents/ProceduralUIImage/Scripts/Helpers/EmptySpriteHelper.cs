using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Helpers
{
    internal static class EmptySpriteHelper
    {
        private static Sprite _sprite;

        internal static Sprite GetSprite()
        {
            if (!_sprite)
                _sprite = Resources.Load<Sprite>("procedural_ui_image_default_sprite");

            return _sprite;
        }

        internal static bool IsEmptySprite(Sprite sprite) => GetSprite() == sprite;
    }
}