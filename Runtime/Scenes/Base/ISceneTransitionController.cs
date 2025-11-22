using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Scenes.Base
{
    [UsedImplicitly]
    public interface ISceneTransitionController
    {
        [UsedImplicitly]
        UniTask StartTransition(string loadingSceneAddress, string destinationSceneAddress);

        [UsedImplicitly]
        void EndTransition();
    }
}