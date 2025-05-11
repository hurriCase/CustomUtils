using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace CustomUtils.Tests.Runtime.CustomUtils.Tests.Runtime
{
    [Resource(name: TestsConfig.CreatedTestScriptableObjectName, resourcePath: TestsConfig.ConfigsPath)]
    internal sealed class CreatedTestScriptableObject : ScriptableObject
    {
        internal static string TestString => TestsConfig.TestString;
    }
}