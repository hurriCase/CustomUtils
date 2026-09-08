using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

using System.Collections.Generic;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier.ShaderGradient
{
    [Resource(name: nameof(GradientConfig))]
    internal sealed class GradientConfig : SingletonScriptableObject<GradientConfig>
    {
        [field: SerializeField] internal Dictionary<GradientType, string> GradientKeywords { get; set; }
    }
}