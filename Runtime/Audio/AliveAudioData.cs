using CustomUtils.Runtime.Audio.Containers;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    internal struct AliveAudioData
    {
        internal SoundType SoundType { get; set; }
        internal AudioSource AudioSource { get; set; }
    }
}