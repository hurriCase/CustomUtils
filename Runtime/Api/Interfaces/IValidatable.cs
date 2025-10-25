using JetBrains.Annotations;

namespace CustomUtils.Runtime.Api.Interfaces
{
    /// <summary>
    /// Defines validation capability for data structures.
    /// </summary>
    [PublicAPI]
    public interface IValidatable
    {
        /// <summary>
        /// Validates the current state of the object.
        /// </summary>
        /// <returns>True if valid; otherwise, false.</returns>
        bool IsValid();
    }
}