using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.Providers;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Storage
{
    /// <summary>
    /// Provides platform-specific storage provider instances.
    /// Automatically selects the appropriate provider based on the current platform.
    /// </summary>
    [UsedImplicitly]
    public static class ServiceProvider
    {
        /// <summary>
        /// Gets the current storage provider instance.
        /// Returns a platform-specific provider or a custom provider if one was set using SetProvider.
        /// </summary>
        /// <returns>The active storage provider instance.</returns>
        [UsedImplicitly]
        public static IStorageProvider Provider => _provider ?? (_provider = GetProvider());
        private static IStorageProvider _provider;

        /// <summary>
        /// Sets a custom storage provider. Must be called before any storage operations.
        /// </summary>
        /// <param name="provider">Custom storage provider implementation</param>
        [UsedImplicitly]
        public static void SetProvider(IStorageProvider provider)
        {
            _provider = provider;
        }

        private static IStorageProvider GetProvider()
        {
            return
#if YANDEX && !UNITY_EDITOR && UNITY_WEBGL
                new PlayerPrefsProvider();
#elif ANDROID && !UNITY_EDITOR && UNITY_ANDROID
                new BinaryFileProvider();
#elif UNITY_EDITOR
                new PlayerPrefsProvider();
#else
                new PlayerPrefsProvider();
#endif
        }
    }
}