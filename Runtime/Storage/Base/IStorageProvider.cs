using System.Threading;
using Cysharp.Threading.Tasks;

namespace CustomUtils.Runtime.Storage.Base
{
    public interface IStorageProvider
    {
        UniTask<bool> SaveAsync<T>(string key, T data, CancellationToken cancellationToken = default);
        UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default);
        UniTask<bool> HasKeyAsync(string key, CancellationToken cancellationToken = default);
        UniTask<bool> TryDeleteKeyAsync(string key, CancellationToken cancellationToken = default);
    }
}