using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.DataTransformers;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Storage.Providers
{
    /// <summary>
    /// PlayerPrefs provider with TryDeleteAll support
    /// </summary>
    [UsedImplicitly]
    internal sealed class PlayerPrefsProvider : BaseStorageProvider
    {
        public PlayerPrefsProvider() : base(new StringDataTransformer()) { }

        protected override UniTask PlatformSaveAsync(string key, object transformData, CancellationToken cancellationToken)
        {
            if (transformData is not string serializedString)
                return UniTask.CompletedTask;

            PlayerPrefs.SetString(key, serializedString);
            PlayerPrefs.Save();

            return UniTask.CompletedTask;
        }

        protected override UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken)
            => UniTask.FromResult<object>(PlayerPrefs.GetString(key, null));

        protected override UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken)
            => UniTask.FromResult(PlayerPrefs.HasKey(key));

        protected override UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();

            return UniTask.CompletedTask;
        }

        protected override UniTask<bool> PlatformTryDeleteAllAsync(CancellationToken cancellationToken)
        {
            PlayerPrefs.DeleteAll();
            return UniTask.FromResult(true);
        }
    }
}