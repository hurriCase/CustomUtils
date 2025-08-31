using CustomUtils.Runtime.UI.Theme.ThemeMapping;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Mappings
{
    [UsedImplicitly]
    [CreateAssetMenu(fileName = nameof(ToggleColorMapping), menuName = nameof(ToggleColorMapping))]
    public sealed class ToggleColorMapping : ThemeStateMappingGeneric<ToggleStateType> { }
}