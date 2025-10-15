namespace CustomUtils.Runtime.Storage.Base
{
    internal interface IDataTransformer
    {
        object TransformForStorage(byte[] data);
        byte[] TransformFromStorage(object storedData);
    }
}