using UnityEngine;

namespace CustomUtils.Runtime.AssetLoader.DontDestroyLoader
{
    /// <summary>
    /// Marker component to identify GameObjects that should persist between scenes.
    /// Add this component to any prefab that should be automatically instantiated
    /// and marked as DontDestroyOnLoad at runtime.
    /// </summary>
    public sealed class DontDestroyOnLoadComponent : MonoBehaviour { }
}