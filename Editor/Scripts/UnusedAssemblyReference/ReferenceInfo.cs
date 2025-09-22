using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CustomUtils.Editor.Scripts.UnusedAssemblyReference
{
    internal sealed class ReferenceInfo
    {
        internal string Name { get; set; }
        internal List<Regex> Patterns { get; set; }
    }
}