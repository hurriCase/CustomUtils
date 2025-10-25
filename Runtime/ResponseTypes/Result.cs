using JetBrains.Annotations;
using UnityEditor;

namespace CustomUtils.Runtime.ResponseTypes
{
    /// <summary>
    /// Result of a validation or operation with a success / failure state.
    /// </summary>
    [UsedImplicitly]
    public readonly struct Result
    {
        /// <summary>
        /// True if the operation was successful.
        /// </summary>
        [UsedImplicitly]
        public bool IsValid { get; }

        /// <summary>
        /// Error message when the operation failed, or null when successful.
        /// </summary>
        [UsedImplicitly]
        public string Message { get; }

        /// <summary>
        /// Creates a valid result indicating successful operation.
        /// </summary>
        [UsedImplicitly]
        public static Result Valid(string message = null) => new(true, message);

        /// <summary>
        /// Creates an invalid result with the specified error message.
        /// </summary>
        [UsedImplicitly]
        public static Result Invalid(string message = null) => new(false, message);

        /// <summary>
        /// Implicitly converts to boolean (true if valid).
        /// </summary>
        public static implicit operator bool(Result result) => result.IsValid;

        /// <summary>
        /// Implicitly converts to error message string.
        /// </summary>
        public static implicit operator string(Result result) => result.Message;

        [UsedImplicitly]
        public Result(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        internal void DisplayMessage()
        {
            var title = IsValid ? "Success" : "Error";
            EditorUtility.DisplayDialog(title, Message, "OK");
        }
    }
}