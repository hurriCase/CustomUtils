using System.Threading;
using CustomUtils.Runtime.Scenes.Base;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
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
        private SceneInstance _currentScene;

        private readonly ISceneLoader _sceneLoader;

        public SceneTransitionController(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public async UniTask StartTransition(
            SceneReference transitionScene,
            SceneReference destinationScene,
            bool isEndAfterTransition = true)
        {
            IsLoading = true;

            _transitionsScene = await _sceneLoader.LoadSceneAsync(
                transitionScene.Address,
                CancellationToken.None,
                LoadSceneMode.Additive);

            if (_currentScene.Scene.IsValid())
                _sceneLoader.TryUnloadScene(_currentScene);

            _currentScene = await _sceneLoader.LoadSceneAsync(destinationScene.Address, CancellationToken.None);

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