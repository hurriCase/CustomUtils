using System;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader
{
    public sealed class ResourceAttribute : Attribute
    {
        public string FullPath { get; }
        public string Name { get; }
        private string ResourcePath { get; }

        public ResourceAttribute(string fullPath = "", string name = "", string resourcePath = "")
        {
            FullPath = fullPath;
            Name = name;
            ResourcePath = resourcePath;
        }

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
    }
}