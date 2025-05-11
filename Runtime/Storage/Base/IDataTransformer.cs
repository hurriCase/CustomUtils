using System.Threading;
using Cysharp.Threading.Tasks;

namespace CustomUtils.Runtime.Storage.Base
{
    public interface IDataTransformer
    {
        UniTask<object> TransformForStorage(byte[] data, CancellationToken cancellationToken);

        UniTask<byte[]> TransformFromStorage(object storedData, CancellationToken cancellationToken);
    }
}