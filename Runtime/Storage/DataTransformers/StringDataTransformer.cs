using System;
using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;

namespace CustomUtils.Runtime.Storage.DataTransformers
{
    public sealed class StringDataTransformer : IDataTransformer
    {
        public UniTask<object> TransformForStorage(byte[] data, CancellationToken cancellationToken)
            => UniTask.FromResult<object>(Convert.ToBase64String(data));

        public UniTask<byte[]> TransformFromStorage(object storedData, CancellationToken cancellationToken)
        {
            if (storedData is string base64String && string.IsNullOrEmpty(base64String) is false)
                return UniTask.FromResult(Convert.FromBase64String(base64String));

            return UniTask.FromResult(Array.Empty<byte>());
        }
    }
}