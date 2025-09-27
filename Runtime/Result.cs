using JetBrains.Annotations;

namespace CustomUtils.Runtime
{
    /// <summary>
    /// Represents the result of a validation or operation with success/failure state.
    /// </summary>
    [UsedImplicitly]
    public readonly struct Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        [UsedImplicitly]
        public bool IsValid { get; }

        /// <summary>
        /// Gets the error message when the operation failed, or null when successful.
        /// </summary>
        [UsedImplicitly]
        public string ErrorMessage { get; }

        /// <summary>
        /// Creates a valid result indicating successful operation.
        /// </summary>
        /// <returns>A valid result with IsValid set to true.</returns>
        [UsedImplicitly]
        public static Result Valid() => new(true, null);

        /// <summary>
        /// Creates an invalid result with the specified error message.
        /// </summary>
        /// <param name="error">The error message describing why the operation failed.</param>
        /// <returns>An invalid result with IsValid set to false and the provided error message.</returns>
        [UsedImplicitly]
        public static Result Invalid(string error) => new(false, error);

        /// <summary>
        /// Implicitly converts a <see cref="Result"/> instance to a boolean value.
        /// </summary>
        /// <param name="result">The result to evaluate.</param>
        /// <returns>True if the result is valid; otherwise, false.</returns>
        public static implicit operator bool(Result result) => result.IsValid;

        /// <summary>
        /// Implicitly converts a <see cref="Result"/> instance to a string value.
        /// </summary>
        /// <param name="result">The result whose error message is to be retrieved.</param>
        /// <returns>The error message if the result is invalid; otherwise, null.</returns>
        public static implicit operator string(Result result) => result.ErrorMessage;

        /// <summary>
        /// Represents the result of a validation or operation with success or failure state.
        /// </summary>
        public Result(bool isValid, string errorMessage)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}