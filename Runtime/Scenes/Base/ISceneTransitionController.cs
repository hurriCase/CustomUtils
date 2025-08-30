using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Scenes.Base
{
    [UsedImplicitly]
    internal interface ISceneTransitionController
    {
        [UsedImplicitly]
        UniTask StartTransition(string transitionSceneAddress, string destinationSceneAddress);

        [UsedImplicitly]
        void EndTransition();
    }
}