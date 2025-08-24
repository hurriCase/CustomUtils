using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CustomUtils.Runtime.Scenes
{
    /// <summary>
    /// Interface for loading and unloading Addressable scenes.
    /// </summary>
    [UsedImplicitly]
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads an Addressable scene asynchronously.
        /// </summary>
        /// <param name="sceneAddress">Addressable scene address to load.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="loadMode">Scene loading mode. Defaults to Single.</param>
        UniTask LoadSceneAsync(string sceneAddress, CancellationToken token,
            LoadSceneMode loadMode = LoadSceneMode.Single);

        /// <summary>
        /// Attempts to unload the specified scene instance.
        /// </summary>
        /// <param name="sceneInstance">Scene instance to unload.</param>
        void TryUnloadScene(SceneInstance sceneInstance);
    }
}