using CustomUtils.Runtime.Animations.Base;
using UnityEngine;

namespace CustomUtils.Runtime.Animations.Settings
{
    [CreateAssetMenu(
        fileName = nameof(Vector3AnimationSettings),
        menuName = AnimationSettingsPath + nameof(Vector3AnimationSettings)
    )]
    public sealed class Vector3AnimationSettings : AnimationSettings<Vector3> { }
}