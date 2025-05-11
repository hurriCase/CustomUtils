using Cysharp.Threading.Tasks;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.Storage.Base
{
    public interface IPersistentProperty
    {
        UniTask SaveAsync();
    }
}