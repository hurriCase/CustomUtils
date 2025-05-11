using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CustomUtils.Editor.UnusedAssemblyReference
{
    internal sealed class ReferenceInfo
    {
        internal string Name { get; set; }
        internal List<Regex> Patterns { get; set; }
    }
}