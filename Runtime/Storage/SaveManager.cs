using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.Providers;

namespace CustomUtils.Runtime.Storage
{
    public sealed class SaveManager
    {
        public static IStorageProvider GetProvider()
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