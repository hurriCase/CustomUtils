using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Attributes
{
    [UsedImplicitly]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SelfAttribute : PropertyAttribute { }
}