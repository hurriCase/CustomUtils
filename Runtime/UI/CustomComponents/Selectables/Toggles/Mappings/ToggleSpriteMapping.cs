using CustomUtils.Runtime.CustomTypes.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Mappings
{
    [UsedImplicitly]
    [CreateAssetMenu(
        fileName = nameof(ToggleSpriteMapping),
        menuName = ResourcePaths.MappingsPath + nameof(ToggleSpriteMapping)
    )]
    public sealed class ToggleSpriteMapping : ScriptableObject
    {
        [field: SerializeField] public EnumArray<ToggleStateType, Sprite> StateMappings { get; private set; }

        [UsedImplicitly]
        public Sprite GetSpriteForState(ToggleStateType state) => StateMappings[state];
    }
}