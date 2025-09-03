using System;
using System.Collections.Generic;
using System.Reflection;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Modifiers;

namespace CustomUtils.Editor.UI.CustomComponents.ProceduralUIImage
{
    internal static class ModifierUtility
    {
        internal static Type GetTypeWithId(string id)
        {
            foreach (var type in Assembly.GetAssembly(typeof(ProceduralImageModifier)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(ProceduralImageModifier)) is false)
                    continue;

                if (((ModifierIDAttribute[])type
                        .GetCustomAttributes(typeof(ModifierIDAttribute), false))[0].Name == id)
                    return type;
            }

            return null;
        }

        internal static List<ModifierIDAttribute> GetAttributeList()
        {
            var result = new List<ModifierIDAttribute>();
            foreach (var type in Assembly.GetAssembly(typeof(ProceduralImageModifier)).GetTypes())
            {
                if (type.IsSubclassOf(typeof(ProceduralImageModifier)))
                    result.Add(((ModifierIDAttribute[])type
                        .GetCustomAttributes(typeof(ModifierIDAttribute), false))[0]);
            }

            return result;
        }
    }
}