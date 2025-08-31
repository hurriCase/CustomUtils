using CustomUtils.Runtime.UI.Theme.ThemeMapping;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [UsedImplicitly]
    [CreateAssetMenu(
        fileName = nameof(SelectableColorMapping),
        menuName = ResourcePaths.MappingsPath + nameof(SelectableColorMapping)
    )]
    public sealed class SelectableColorMapping : ThemeStateMappingGeneric<SelectableStateType> { }
}