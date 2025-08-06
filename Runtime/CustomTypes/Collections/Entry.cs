using System;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    // For created to support serialization for nested type without any issues
    [Serializable]
    public struct Entry<TValue>
    {
        [field: SerializeField] public TValue Value { get; set; }
    }
}