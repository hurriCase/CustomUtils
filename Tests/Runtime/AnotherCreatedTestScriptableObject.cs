using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace CustomUtils.Tests.Runtime.CustomUtils.Tests.Runtime
{
    [Resource(name: "AnotherCreatedTestScriptableObject", resourcePath: "Configs")]
    internal sealed class AnotherCreatedTestScriptableObject : ScriptableObject
    {
        internal static string AnotherTestString => TestsConfig.AnotherTestString;
    }
}