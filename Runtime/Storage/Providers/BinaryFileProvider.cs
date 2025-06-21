using System.IO;
using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.DataTransformers;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Storage.Providers
{
    /// <summary>
    /// Binary file provider with TryDeleteAll support
    /// </summary>
    [UsedImplicitly]
    internal sealed class BinaryFileProvider : BaseStorageProvider
    {
        private readonly string _saveDirectory;

        internal BinaryFileProvider() : base(new IdentityDataTransformer())
        {
            _saveDirectory = Path.Combine(Application.persistentDataPath, "SaveData");

            if (Directory.Exists(_saveDirectory) is false)
                Directory.CreateDirectory(_saveDirectory);
        }

        private string GetFilePath(string key) => Path.Combine(_saveDirectory, $"{key}.dat");

        protected override async UniTask PlatformSaveAsync(string key, object transformData, CancellationToken cancellationToken)
        {
            if (transformData is byte[] byteData)
                await File.WriteAllBytesAsync(GetFilePath(key), byteData, cancellationToken);
        }

        protected override async UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken)
        {
            var filePath = GetFilePath(key);

            if (File.Exists(filePath) is false)
                return null;

            var result = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return result;
        }

        protected override UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken)
            => UniTask.FromResult(File.Exists(GetFilePath(key)));

        protected override UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            var filePath = GetFilePath(key);

            return UniTask.RunOnThreadPool(
                static path =>
                {
                    if (File.Exists((string)path))
                        File.Delete((string)path);
                },
                filePath,
                configureAwait: true,
                cancellationToken: cancellationToken);
        }

        protected override UniTask<bool> PlatformTryDeleteAllAsync(CancellationToken cancellationToken)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                if (Directory.Exists(_saveDirectory) is false)
                    return true;

                Directory.Delete(_saveDirectory, true);
                Directory.CreateDirectory(_saveDirectory);
                return true;
            }, configureAwait: true, cancellationToken: cancellationToken);
        }
    }
}