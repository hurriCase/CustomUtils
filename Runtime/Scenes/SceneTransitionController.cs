using System.Threading;
using CustomUtils.Runtime.Scenes.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CustomUtils.Runtime.Scenes
{
    [UsedImplicitly]
    internal sealed class SceneTransitionController : ISceneTransitionController
    {
        private readonly ISceneLoader _sceneLoader;

        private SceneInstance _transitionsScene;

        internal SceneTransitionController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartTransition(string transitionSceneAddress, string destinationSceneAddress)
        {
            _transitionsScene = await _sceneLoader.LoadSceneAsync(transitionSceneAddress, CancellationToken.None);

            _sceneLoader.LoadSceneAsync(destinationSceneAddress, CancellationToken.None, LoadSceneMode.Additive)
                .Forget();
        }

        public void EndTransition()
        {
            _sceneLoader.TryUnloadScene(_transitionsScene);
        }
    }
}