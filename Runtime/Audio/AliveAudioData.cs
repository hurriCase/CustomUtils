using System;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    internal struct AliveAudioData<TEnum> where TEnum : unmanaged, Enum
    {
        internal TEnum SoundType { get; set; }
        internal AudioSource AudioSource { get; set; }
    }
}