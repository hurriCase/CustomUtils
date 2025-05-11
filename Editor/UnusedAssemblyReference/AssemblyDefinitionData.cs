using System;

namespace CustomUtils.Editor.UnusedAssemblyReference
{
    [Serializable]
    internal class AssemblyDefinitionData
    {
        internal string Name { get; private set; }
        internal string RootNamespace { get; private set; }
        internal string[] References { get; private set; }
    }
}