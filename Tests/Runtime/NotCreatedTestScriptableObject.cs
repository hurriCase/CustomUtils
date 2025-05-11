using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace CustomUtils.Tests.Runtime.CustomUtils.Tests.Runtime
{
    [Resource(name: TestsConfig.NotCreatedTestScriptableObjectName, resourcePath: TestsConfig.ConfigsPath)]
    internal sealed class NotCreatedTestScriptableObject : ScriptableObject
    {
        internal string TestString => TestsConfig.TestString;
    }
}