using System;
using CustomUtils.Runtime.Storage.Base;

namespace CustomUtils.Runtime.Storage.DataTransformers
{
    internal sealed class StringDataTransformer : IDataTransformer
    {
        public object TransformForStorage(byte[] data) => Convert.ToBase64String(data);

        public byte[] TransformFromStorage(object storedData)
            => storedData is string str ? Convert.FromBase64String(str) : Array.Empty<byte>();
    }
}