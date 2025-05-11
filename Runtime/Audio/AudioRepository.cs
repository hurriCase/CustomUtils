using CustomUtils.Runtime.Storage;

namespace CustomUtils.Runtime.Audio
{
    public abstract class AudioRepository
    {
        public abstract PersistentReactiveProperty<float> MusicVolume { get; }
        public abstract PersistentReactiveProperty<float> SoundVolume { get; }
    }
}