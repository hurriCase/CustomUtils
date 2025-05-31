using System;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using UnityEngine;

namespace CustomUtils.Runtime.Audio.Containers
{
    [Serializable]
    public class AudioContainerBase<T> where T : unmanaged, Enum
    {
        [field: SerializeField] public T AudioType { get; private set; }
        [field: SerializeField] public AudioData AudioData { get; private set; }

        [NonSerialized] private int _cachedId = -1;

        /// <summary>
        /// Gets the cached integer ID for this audio type
        /// </summary>
        /// <returns>Integer representation of the audio type</returns>
        internal int GetId()
        {
            if (_cachedId == -1)
                _cachedId = UnsafeEnumConverter<T>.ToInt32(AudioType);

            return _cachedId;
        }
    }
}