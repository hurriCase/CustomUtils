using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Scenes.Base
{
    [PublicAPI]
    public interface ISceneTransitionController
    {
        bool IsLoading { get; }

        UniTask StartTransition(
            string loadingSceneAddress,
            string destinationSceneAddress,
            bool isEndAfterTransition = true);

        UniTask EndTransition();
    }
}