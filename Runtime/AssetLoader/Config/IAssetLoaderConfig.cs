namespace CustomUtils.Runtime.AssetLoader.Config
{
    /// <summary>
    /// Configuration interface for Asset Loader system.
    /// Allows customization of asset loading paths.
    /// </summary>
    public interface IAssetLoaderConfig
    {
        /// <summary>
        /// Gets the path where DontDestroyOnLoad prefabs are stored in Resources folder.
        /// </summary>
        string DontDestroyPath { get; }
    }
}