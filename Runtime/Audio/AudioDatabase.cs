using System.Collections.Generic;
using CustomUtils.Runtime.Audio.Containers;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    [CreateAssetMenu(
        fileName = nameof(AudioDatabase),
        menuName = MenuNames.DatabasePath + nameof(AudioDatabase)
    )]
    public sealed class AudioDatabase : ScriptableObject
    {
        [field: SerializeField] public List<SoundContainer> SoundContainers { get; private set; }
        [field: SerializeField] public List<MusicContainer> MusicContainers { get; private set; }

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        internal SoundContainer GetSoundContainer(SoundType soundType)
        {
            foreach (var soundContainer in SoundContainers)
            {
                if (soundContainer.SoundType == soundType)
                    return soundContainer;
            }

            return null;
        }

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        internal MusicContainer GeMusicContainer(MusicType musicType)
        {
            foreach (var musicContainer in MusicContainers)
            {
                if (musicContainer.MusicType == musicType)
                    return musicContainer;
            }

            return null;
        }
    }
}