using System;
using UnityEngine;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute that makes a field read-only in the Unity Inspector.
    /// Can be applied to any serialized field to prevent editing while still showing the value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InspectorReadOnlyAttribute : PropertyAttribute { }
}