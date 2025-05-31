﻿using System;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Storage.DataTransformers
{
    internal sealed class IdentityDataTransformer : IDataTransformer
    {
        public object TransformForStorage(byte[] data) => data;
        public byte[] TransformFromStorage(object storedData) => storedData as byte[] ?? Array.Empty<byte>();
    }
}