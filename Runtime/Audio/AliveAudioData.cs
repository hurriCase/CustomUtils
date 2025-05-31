using System;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    internal struct AliveAudioData<T> where T : unmanaged, Enum
    {
        internal T SoundType { get; set; }
        internal AudioSource AudioSource { get; set; }
    }
}