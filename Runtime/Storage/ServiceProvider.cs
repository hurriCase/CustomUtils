using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.Providers;

namespace CustomUtils.Runtime.Storage
{
    internal static class ServiceProvider
    {
        internal static IStorageProvider Provider => _provider ?? (_provider = GetProvider());
        private static IStorageProvider _provider;

        private static IStorageProvider GetProvider()
        {
            return
#if YANDEX && !UNITY_EDITOR && UNITY_WEBGL
                new PlayerPrefsProvider();
#elif ANDROID && !UNITY_EDITOR && UNITY_ANDROID
                new BinaryFileProvider();
#elif UNITY_EDITOR
                new PlayerPrefsProvider();
#endif
        }
    }
}