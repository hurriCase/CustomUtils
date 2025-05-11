using Cysharp.Threading.Tasks;

namespace CustomUtils.Runtime.Storage.Base
{
    public interface IPersistentProperty
    {
        UniTask SaveAsync();
    }
}