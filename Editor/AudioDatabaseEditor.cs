using System.Collections.Generic;
using CustomUtils.Runtime.Audio;
using CustomUtils.Runtime.Audio.Containers;
using UnityEditor;
using ZLinq;

namespace CustomUtils.Editor
{
    [CustomEditor(typeof(AudioDatabase))]
    internal sealed class AudioDatabaseEditor : EnumDrivenHandlerEditor<AudioDatabase, SoundContainer, SoundType>
    {
        protected override string EnumFieldName => nameof(SoundContainer.SoundType);

        protected override List<SoundType> GetUsedEnumValues(AudioDatabase database)
            => database.SoundContainers.AsValueEnumerable().Select(enemy => enemy.SoundType).ToList();
    }
}