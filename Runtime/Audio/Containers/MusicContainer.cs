using System;

namespace CustomUtils.Runtime.Audio.Containers
{
    [Serializable]
    public sealed class MusicContainer<T> : AudioContainerBase<T> where T : unmanaged, Enum { }
}