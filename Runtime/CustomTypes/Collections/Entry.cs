using System;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    // Was created to support serialization for nested types without any issues
    [Serializable]
    public struct Entry<TValue>
    {
        [field: SerializeField] public TValue Value { get; set; }
    }
}