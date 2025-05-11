using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomUtils.Runtime.Audio
{
    [Serializable]
    public class AudioData
    {
        [field: SerializeField] internal AudioClip AudioClip { get; private set; }

        [SerializeField] private float _minRandomVolume = 1f;
        [SerializeField] private float _maxRandomVolume = 1f;

        [SerializeField] private float _minRandomPitch = 1f;
        [SerializeField] private float _maxRandomPitch = 1f;

        internal float RandomPitch => Random.Range(_minRandomPitch, _maxRandomPitch);
        internal float RandomVolume => Random.Range(_minRandomVolume, _maxRandomVolume);
    }
}