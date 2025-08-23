using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute that conditionally hides a field or property in the Unity Inspector based on the value of another field.
    /// Can be applied to any serialized field or property to create dynamic Inspector behavior.
    /// </summary>
    /// <remarks>
    /// This attribute allows you to show or hide fields based on boolean conditions from other fields,
    /// creating more dynamic and user-friendly Inspector layouts. The conditional field must be a boolean
    /// field within the same class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [UsedImplicitly]
    public sealed class HideIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets the name of the field that controls the visibility of the decorated field.
        /// </summary>
        /// <remarks>
        /// This field must be a boolean field within the same class as the decorated field.
        /// </remarks>
        [UsedImplicitly]
        public string ConditionalSourceField { get; private set; }

        /// <summary>
        /// Gets the hide type that determines when the field should be hidden.
        /// </summary>
        /// <value>
        /// A <see cref="HideType"/> value that specifies the condition for hiding the field.
        /// </value>
        [UsedImplicitly]
        public HideType HideType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HideIfAttribute"/> class.
        /// </summary>
        /// <param name="conditionalSourceField">
        /// The name of the boolean field that controls the visibility of the decorated field.
        /// This field must exist within the same class.
        /// </param>
        /// <param name="hideType">
        /// Specifies when to hide the field. Default is <see cref="HideType.False"/>.
        /// </param>
        [UsedImplicitly]
        public HideIfAttribute([NotNull] string conditionalSourceField, HideType hideType = HideType.False)
        {
            ConditionalSourceField = conditionalSourceField;
            HideType = hideType;
        }
    }

    /// <summary>
    /// Specifies the condition under which a field should be hidden in the Unity Inspector.
    /// </summary>
    public enum HideType
    {
        /// <summary>
        /// No hiding behavior (field is always shown).
        /// </summary>
        None = 0,

        /// <summary>
        /// Hide the field when the conditional source field is true.
        /// </summary>
        True = 1,

        /// <summary>
        /// Hide the field when the conditional source field is false.
        /// </summary>
        False = 2
    }
}