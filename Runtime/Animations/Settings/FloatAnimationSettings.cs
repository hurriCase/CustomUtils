using CustomUtils.Runtime.Animations.Base;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Settings
{
    [CreateAssetMenu(
        fileName = nameof(FloatAnimationSettings),
        menuName = AnimationSettingsPath + nameof(FloatAnimationSettings)
    )]
    public sealed class FloatAnimationSettings : AnimationSettings<float> { }
}