using System;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute for specifying the location of a resource asset.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ResourceAttribute : Attribute
    {
        /// <summary>
        /// Gets the full path to the resource asset folder.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Gets the name of the resource asset.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the resource path used for loading with Resources.Load.
        /// </summary>
        private string ResourcePath { get; }

        /// <summary>
        /// Gets a value indicating whether this resource is in the Editor Default Resources folder.
        /// </summary>
        public bool IsEditorResource { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CustomUtils.Runtime.AssetLoader.ResourceAttribute" /> class.
        /// </summary>
        /// <param name="fullPath">The full path to the resource asset folder.</param>
        /// <param name="name">The name of the resource asset.</param>
        /// <param name="resourcePath">The resource path used for loading with Resources.Load.</param>
        /// <param name="isEditorResource">Indicates whether this resource is in the Editor Default Resources folder.</param>
        public ResourceAttribute(string fullPath = "", string name = "", string resourcePath = "",
            bool isEditorResource = false)
        {
            FullPath = fullPath;
            Name = name;
            ResourcePath = resourcePath;
            IsEditorResource = isEditorResource;
        }

        /// <summary>
        /// Tries to get the full resource path for use with Resources.Load.
        /// </summary>
        /// <param name="fullResourcePath">When this method returns, contains the full resource path if successful; otherwise, null.</param>
        /// <returns>true if the full resource path was successfully created; otherwise, false.</returns>
        public bool TryGetFullResourcePath(out string fullResourcePath)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                fullResourcePath = null;
                return false;
            }

            fullResourcePath = string.IsNullOrWhiteSpace(ResourcePath) ? Name : $"{ResourcePath}/{Name}";
            return true;
        }

        /// <summary>
        /// Gets the editor resource path for use with EditorGUIUtility.Load.
        /// </summary>
        /// <returns>The path suitable for editor resource loading, or null if not an editor resource.</returns>
        public string GetEditorResourcePath()
        {
            if (IsEditorResource is false || string.IsNullOrWhiteSpace(Name))
                return null;

            return string.IsNullOrWhiteSpace(FullPath)
                ? Name
                : $"{FullPath}/{Name}";
        }
    }
}