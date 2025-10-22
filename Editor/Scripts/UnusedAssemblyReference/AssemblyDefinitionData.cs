using System;

namespace CustomUtils.Editor.Scripts.UnusedAssemblyReference
{
    [Serializable]
    internal class AssemblyDefinitionData
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local | setter is used by Json
        internal string Name { get; private set; }
        internal string RootNamespace { get; private set; }
        internal string[] References { get; private set; }
    }
}