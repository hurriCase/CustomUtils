using System;
using UnityEngine;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Marks a field as required. Fields with this attribute must be assigned in the Unity Inspector.
    /// </summary>
    /// <remarks>
    /// During runtime initialization, fields with this attribute will be validated, and error messages
    /// will be logged if they are null or empty.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RequiredFieldAttribute : PropertyAttribute { }
}