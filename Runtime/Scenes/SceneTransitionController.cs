using System.Threading;
using CustomUtils.Runtime.Scenes.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CustomUtils.Runtime.Scenes
{
    [UsedImplicitly]
    public sealed class SceneTransitionController : ISceneTransitionController
    {
        private readonly ISceneLoader _sceneLoader;

        private SceneInstance _loadingScene;

        public SceneTransitionController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartTransition(string loadingSceneAddress, string destinationSceneAddress)
        {
            _loadingScene = await _sceneLoader.LoadSceneAsync(loadingSceneAddress, CancellationToken.None);

            _sceneLoader.LoadSceneAsync(destinationSceneAddress, CancellationToken.None, LoadSceneMode.Additive)
                .Forget();
        }

        public void EndTransition()
        {
            _sceneLoader.TryUnloadScene(_loadingScene);
        }
    }
}