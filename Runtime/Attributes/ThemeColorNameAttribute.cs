using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Attributes
{
    [UsedImplicitly]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ThemeColorNameAttribute : PropertyAttribute { }
}