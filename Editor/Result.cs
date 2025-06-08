using JetBrains.Annotations;

namespace CustomUtils.Editor
{
    /// <summary>
    /// Represents the result of a validation or operation with success/failure state.
    /// </summary>
    [UsedImplicitly]
    public struct Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        [UsedImplicitly]
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets the error message when the operation failed, or null when successful.
        /// </summary>
        [UsedImplicitly]
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Creates a valid result indicating successful operation.
        /// </summary>
        /// <returns>A valid result with IsValid set to true.</returns>
        [UsedImplicitly]
        public static Result Valid() => new() { IsValid = true, ErrorMessage = null };

        /// <summary>
        /// Creates an invalid result with the specified error message.
        /// </summary>
        /// <param name="error">The error message describing why the operation failed.</param>
        /// <returns>An invalid result with IsValid set to false and the provided error message.</returns>
        [UsedImplicitly]
        public static Result Invalid(string error) => new() { IsValid = false, ErrorMessage = error };
    }
}