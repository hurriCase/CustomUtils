using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [UsedImplicitly]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ThemeColorNameAttribute : PropertyAttribute { }
}