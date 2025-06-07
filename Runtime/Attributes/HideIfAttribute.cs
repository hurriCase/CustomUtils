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
        /// Gets a value indicating whether the field should be hidden when the conditional source field is true.
        /// </summary>
        /// <value>
        /// <c>true</c> to hide the field when the source field is true; <c>false</c> to hide when the source field is false.
        /// </value>
        [UsedImplicitly]
        public bool HideIfSourceTrue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HideIfAttribute"/> class.
        /// </summary>
        /// <param name="conditionalSourceField">
        /// The name of the boolean field that controls the visibility of the decorated field.
        /// This field must exist within the same class.
        /// </param>
        /// <param name="hideIfSourceTrue">
        /// If <c>true</c>, hides the decorated field when the source field is true.
        /// If <c>false</c>, hides the decorated field when the source field is false.
        /// Default is <c>false</c>.
        /// </param>
        [UsedImplicitly]
        public HideIfAttribute([NotNull] string conditionalSourceField, bool hideIfSourceTrue = false)
        {
            ConditionalSourceField = conditionalSourceField;
            HideIfSourceTrue = hideIfSourceTrue;
        }
    }
}