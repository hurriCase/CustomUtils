using System;
using UnityEngine;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute that makes an enum field contains only distinct values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class DistinctEnumAttribute : PropertyAttribute { }
}