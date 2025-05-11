using System;
using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Storage.DataTransformers
{
    public sealed class IdentityDataTransformer : IDataTransformer
    {
        public UniTask<object> TransformForStorage(byte[] data, CancellationToken cancellationToken)
            => UniTask.FromResult<object>(data);

        public UniTask<byte[]> TransformFromStorage(object storedData, CancellationToken cancellationToken)
            => UniTask.FromResult(storedData as byte[] ?? Array.Empty<byte>());
    }
}