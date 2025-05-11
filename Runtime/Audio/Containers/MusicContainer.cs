using System;
using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.Audio.Containers
{
    [Serializable]
    public sealed class MusicContainer
    {
        [field: SerializeField, DistinctEnum] public MusicType MusicType { get; private set; }
        [field: SerializeField] public AudioData AudioData { get; private set; }
    }
}