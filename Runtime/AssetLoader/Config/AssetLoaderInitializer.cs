// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader.Config
{
    /// <summary>
    /// Static initializer for Asset Loader system.
    /// Manages configuration and provides access to loader settings.
    /// </summary>
    public static class AssetLoaderInitializer
    {
        internal static IAssetLoaderConfig LoaderConfig { get; private set; } = AssetLoaderConfig.Instance;

        /// <summary>
        /// Initializes the Asset Loader with custom configuration.
        /// </summary>
        /// <param name="loaderConfig">Custom configuration implementing IAssetLoaderConfig.</param>
        public static void Init(IAssetLoaderConfig loaderConfig)
        {
            LoaderConfig = loaderConfig;
        }
    }
}