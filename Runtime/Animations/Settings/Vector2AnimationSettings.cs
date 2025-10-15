using CustomUtils.Runtime.Animations.Base;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Settings
{
    [CreateAssetMenu(
        fileName = nameof(Vector2AnimationSettings),
        menuName = AnimationSettingsPath + nameof(Vector2AnimationSettings)
    )]
    public sealed class Vector2AnimationSettings : AnimationSettings<Vector2> { }
}