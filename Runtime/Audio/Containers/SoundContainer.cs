using System;
using UnityEngine;

namespace CustomUtils.Runtime.Audio.Containers
{
    [Serializable]
    public sealed class SoundContainer<T> : AudioContainerBase<T> where T : unmanaged, Enum
    {
        [field: SerializeField] public float Cooldown { get; private set; }
    }
}