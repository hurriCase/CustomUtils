using UnityEditor;

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
namespace CustomUtils.Editor.Extensions
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Finds a serialized property using its backing field name format.
        /// </summary>
        /// <param name="serializedObject">The serialized object to search in.</param>
        /// <param name="name">The property name to find (will be converted to backing field format).</param>
        /// <returns>The found serialized property or null if not found.</returns>
        public static SerializedProperty FindField(this SerializedObject serializedObject, string name) =>
            serializedObject.FindProperty(name.ConvertToBackingField());

        /// <summary>
        /// Finds a relative serialized property using its backing field name format.
        /// </summary>
        /// <param name="serializedObject">The serialized property to search in.</param>
        /// <param name="name">The property name to find (will be converted to backing field format).</param>
        /// <returns>The found relative serialized property or null if not found.</returns>
        public static SerializedProperty FindFieldRelative(this SerializedProperty serializedObject, string name) =>
            serializedObject.FindPropertyRelative(name.ConvertToBackingField());
    }
}