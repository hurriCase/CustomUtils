using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Scenes.Base
{
    [PublicAPI]
    public interface ISceneTransitionController
    {
        bool IsLoading { get; }

        UniTask StartTransition(
            SceneReference transitionScene,
            SceneReference destinationScene,
            bool isEndAfterTransition = true);

        UniTask EndTransition();
    }
}