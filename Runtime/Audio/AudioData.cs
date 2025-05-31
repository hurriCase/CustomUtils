using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomUtils.Runtime.Audio
{
    /// <summary>
    /// Contains audio clip and randomization settings for sound playback
    /// </summary>
    [Serializable]
    public class AudioData
    {
        /// <summary>
        /// The audio clip to be played
        /// </summary>
        [field: SerializeField] public AudioClip AudioClip { get; private set; }

        [SerializeField] private float _minRandomVolume = 1f;
        [SerializeField] private float _maxRandomVolume = 1f;

        [SerializeField] private float _minRandomPitch = 1f;
        [SerializeField] private float _maxRandomPitch = 1f;

        /// <summary>
        /// Gets a random pitch value between the configured min and max range
        /// </summary>
        /// <returns>Random pitch multiplier</returns>
        public float RandomPitch => Random.Range(_minRandomPitch, _maxRandomPitch);

        /// <summary>
        /// Gets a random volume value between the configured min and max range
        /// </summary>
        /// <returns>Random volume multiplier</returns>
        public float RandomVolume => Random.Range(_minRandomVolume, _maxRandomVolume);
    }
}