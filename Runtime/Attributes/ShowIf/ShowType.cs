using JetBrains.Annotations;

namespace CustomUtils.Runtime.Attributes.ShowIf
{
    /// <summary>
    /// Specifies the condition under which a field should be shown in the Unity Inspector.
    /// </summary>
    [PublicAPI]
    public enum ShowType
    {
        /// <summary>
        /// Show the field when the conditional source field is true.
        /// </summary>
        True = 0,

        /// <summary>
        /// Show the field when the conditional source field is false.
        /// </summary>
        False = 1
    }
}