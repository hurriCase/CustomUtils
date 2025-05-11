using System;
using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.Audio.Containers
{
    [Serializable]
    public sealed class SoundContainer
    {
        [field: SerializeField, DistinctEnum] public SoundType SoundType { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public AudioData AudioData { get; private set; }
    }
}