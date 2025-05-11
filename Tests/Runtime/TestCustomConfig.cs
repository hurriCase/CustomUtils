using CustomUtils.Runtime.AssetLoader.Config;

namespace CustomUtils.Tests.Runtime.CustomUtils.Tests.Runtime
{
    internal sealed class TestCustomConfig : IAssetLoaderConfig
    {
        public string DontDestroyPath => TestsConfig.TestDontDestroyOnLoad;
    }
}