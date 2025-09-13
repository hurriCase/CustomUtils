using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute that conditionally shows a field or property in the Unity Inspector based on the value of another field.
    /// Can be applied to any serialized field or property to create dynamic Inspector behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [UsedImplicitly]
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets the name of the field that controls the visibility of the decorated field.
        /// </summary>
        [UsedImplicitly]
        public string ConditionalSourceField { get; private set; }

        /// <summary>
        /// Gets the show type that determines when the field should be shown.
        /// </summary>
        [UsedImplicitly]
        public ShowType ShowType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ShowIfAttribute class.
        /// </summary>
        /// <param name="conditionalSourceField">
        /// The name of the boolean field that controls the visibility of the decorated field.
        /// This field must exist within the same class.
        /// </param>
        /// <param name="showType">
        /// Specifies when to show the field. Default is ShowType.True.
        /// </param>
        [UsedImplicitly]
        public ShowIfAttribute(string conditionalSourceField, ShowType showType = ShowType.True)
        {
            ConditionalSourceField = conditionalSourceField;
            ShowType = showType;
        }
    }

    /// <summary>
    /// Specifies the condition under which a field should be shown in the Unity Inspector.
    /// </summary>
    [UsedImplicitly]
    public enum ShowType
    {
        /// <summary>
        /// No showing behavior (field visibility unchanged).
        /// </summary>
        [UsedImplicitly]
        None = 0,

        /// <summary>
        /// Show the field when the conditional source field is true.
        /// </summary>
        [UsedImplicitly]
        True = 1,

        /// <summary>
        /// Show the field when the conditional source field is false.
        /// </summary>
        [UsedImplicitly]
        False = 2
    }
}