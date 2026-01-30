#if CRAZY_GAMES
using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.DataTransformers;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using CrazyGames;

namespace CustomUtils.Runtime.Storage.Providers
{
    /// <summary>
    /// PlayerPrefs provider with TryDeleteAll support
    /// </summary>
    [UsedImplicitly]
    internal sealed class CrazyGamesStorageProvider : BaseStorageProvider
    {
        public CrazyGamesStorageProvider() : base(new StringDataTransformer()) { }

        protected override UniTask PlatformSaveAsync(string key, object transformData, CancellationToken cancellationToken)
        {
            if (transformData is not string serializedString)
                return UniTask.CompletedTask;

            CrazySDK.Data.SetString(key, serializedString);

            return UniTask.CompletedTask;
        }

        protected override UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken)
            => UniTask.FromResult<object>(CrazySDK.Data.GetString(key, null));

        protected override UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken)
            => UniTask.FromResult(CrazySDK.Data.HasKey(key));

        protected override UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            CrazySDK.Data.DeleteKey(key);

            return UniTask.CompletedTask;
        }

        protected override UniTask<bool> PlatformTryDeleteAllAsync(CancellationToken cancellationToken)
        {
            CrazySDK.Data.DeleteAll();
            return UniTask.FromResult(true);
        }
    }
}
#endif