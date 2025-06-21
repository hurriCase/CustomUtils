using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.DataTransformers;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Storage.Providers
{
    /// <summary>
    /// Hybrid storage provider with smart TryDeleteAll behavior
    /// Reads from local first, then cloud. Writes to cloud with local fallback.
    /// </summary>
    [UsedImplicitly]
    internal sealed class HybridStorageProvider : BaseStorageProvider
    {
        private readonly IStorageProvider _localProvider;
        private readonly IStorageProvider _cloudProvider;
        private readonly bool _autoMigrate;

        /// <summary>
        /// Creates a hybrid provider that combines local and cloud storage
        /// </summary>
        /// <param name="localProvider">Local storage provider (fallback)</param>
        /// <param name="cloudProvider">Cloud storage provider (primary)</param>
        /// <param name="autoMigrate">Whether to automatically migrate local data to cloud</param>
        public HybridStorageProvider(IStorageProvider localProvider, IStorageProvider cloudProvider,
            bool autoMigrate = true) : base(new IdentityDataTransformer())
        {
            _localProvider = localProvider;
            _cloudProvider = cloudProvider;
            _autoMigrate = autoMigrate;
        }

        protected override async UniTask PlatformSaveAsync(string key, object transformData,
            CancellationToken cancellationToken)
        {
            if (await _cloudProvider.TrySaveAsync(key, transformData, cancellationToken))
                if (_autoMigrate)
                {
                    await _localProvider.TryDeleteKeyAsync(key, cancellationToken);
                    return;
                }

#if IS_TEST
            Debug.LogWarning("[HybridStorageProvider] " +
                             $"Cloud save failed for key '{key}'. Falling back to local.");
#endif

            await _localProvider.TrySaveAsync(key, transformData, cancellationToken);
        }

        protected override async UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken)
        {
            if (await _cloudProvider.HasKeyAsync(key, cancellationToken))
                return await _cloudProvider.LoadAsync<object>(key, cancellationToken);

            if (await _localProvider.HasKeyAsync(key, cancellationToken) is false)
                return null;

            var localData = await _localProvider.LoadAsync<object>(key, cancellationToken);

            if (_autoMigrate is false || localData == null)
                return localData;

            if (await _cloudProvider.TrySaveAsync(key, localData, cancellationToken))
                await _localProvider.TryDeleteKeyAsync(key, cancellationToken);

#if IS_TEST
            Debug.Log($"[HybridStorageProvider::PlatformLoadAsync] Auto-migrated key '{key}' to cloud storage");
#endif

            return localData;
        }

        protected override async UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken)
        {
            if (await _cloudProvider.HasKeyAsync(key, cancellationToken))
                return true;

            return await _localProvider.HasKeyAsync(key, cancellationToken);
        }

        protected override async UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            await _cloudProvider.TryDeleteKeyAsync(key, cancellationToken);
            await _localProvider.TryDeleteKeyAsync(key, cancellationToken);
        }

        protected override async UniTask<bool> PlatformTryDeleteAllAsync(CancellationToken cancellationToken)
        {
            var localSuccess = await _localProvider.TryDeleteAllAsync(cancellationToken);
            var cloudSuccess = await _cloudProvider.TryDeleteAllAsync(cancellationToken);

            var overallSuccess = localSuccess || cloudSuccess;
            return overallSuccess;
        }
    }
}