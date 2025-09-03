using System;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Attributes
{
    [UsedImplicitly]
    [AttributeUsage(AttributeTargets.Class)]
    public class ModifierIDAttribute : Attribute
    {
        [UsedImplicitly]
        public string Name { get; }

        [UsedImplicitly]
        public ModifierIDAttribute(string name)
        {
            Name = name;
        }
    }
}