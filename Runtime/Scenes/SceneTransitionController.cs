using System.Threading;
using CustomUtils.Runtime.Scenes.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CustomUtils.Runtime.Scenes
{
    [PublicAPI]
    public sealed class SceneTransitionController : ISceneTransitionController
    {
        public bool IsLoading { get; private set; }

        private SceneInstance _transitionsScene;

        private readonly ISceneLoader _sceneLoader;

        public SceneTransitionController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartTransition(
            string transitionSceneAddress,
            string destinationSceneAddress,
            bool isEndAfterTransition = true)
        {
            IsLoading = true;

            _transitionsScene = await _sceneLoader.LoadSceneAsync(transitionSceneAddress, CancellationToken.None);

            await _sceneLoader.LoadSceneAsync(destinationSceneAddress, CancellationToken.None, LoadSceneMode.Additive);

            IsLoading = false;

            if (isEndAfterTransition)
                EndTransition().Forget();
        }

        public async UniTask EndTransition()
        {
            await UniTask.WaitUntil(this, static self => self.IsLoading is false);

            _sceneLoader.TryUnloadScene(_transitionsScene);
        }
    }
}