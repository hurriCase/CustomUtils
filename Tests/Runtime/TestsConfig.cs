namespace CustomUtils.Tests.Runtime.CustomUtils.Tests.Runtime
{
    internal static class TestsConfig
    {
        internal const string TestDontDestroyOnLoadObject = "TestDontDestroyOnLoadObject";

        internal const string NotFoundResourceLogWarning =
            "[ResourceLoader::Load] Failed to load resource at path: Configs/NotCreatedTestScriptableObject";

        internal const string CreatedTestScriptableObjectName = "CreatedTestScriptableObject";
        internal const string NotCreatedTestScriptableObjectName = "NotCreatedTestScriptableObject";
        internal const string ExplicitPath = "Configs/CreatedTestScriptableObject";
        internal const string TestDontDestroyOnLoad = "TestDontDestroyOnLoad";
        internal const string ConfigsPath = "Configs";
        internal const string TestString = "TestString";
        internal const string AnotherTestString = "AnotherTestString";
    }
}