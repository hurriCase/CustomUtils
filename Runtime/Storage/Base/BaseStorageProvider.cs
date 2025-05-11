using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MemoryPack;
using UnityEngine;

namespace CustomUtils.Runtime.Storage.Base
{
    public abstract class BaseStorageProvider : IStorageProvider
    {
        private readonly Dictionary<string, byte[]> _cache = new();
        private readonly IDataTransformer _dataTransformer;

        protected BaseStorageProvider(IDataTransformer dataTransformer)
        {
            _dataTransformer = dataTransformer;
        }

        public async UniTask<bool> SaveAsync<T>(string key, T data, CancellationToken cancellationToken)
        {
            try
            {
                var serialized = MemoryPackSerializer.Serialize(data);
                _cache[key] = serialized;

                var transformedData = await _dataTransformer.TransformForStorage(serialized, cancellationToken);

                await PlatformSaveAsync(key, transformedData, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BaseStorageProvider::SaveAsync] Error saving data: {ex.Message}");
                return false;
            }
        }

        public async UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken)
        {
            try
            {
                if (_cache.TryGetValue(key, out var cachedData))
                    return MemoryPackSerializer.Deserialize<T>(cachedData);

                var storedData = await PlatformLoadAsync(key, cancellationToken);
                if (storedData == null)
                    return default;

                var buffer = await _dataTransformer.TransformFromStorage(storedData, cancellationToken);
                if (buffer == null || buffer.Length == 0)
                    return default;

                _cache[key] = buffer;
                return MemoryPackSerializer.Deserialize<T>(buffer);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BaseStorageProvider::LoadAsync] Error loading data: {ex.Message}");
                return default;
            }
        }

        public async UniTask<bool> HasKeyAsync(string key, CancellationToken cancellationToken)
        {
            if (_cache.ContainsKey(key))
                return true;

            return await PlatformHasKeyAsync(key, cancellationToken);
        }

        public async UniTask<bool> TryDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                _cache.Remove(key);
                await PlatformDeleteKeyAsync(key, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BaseStorageProvider::TryDeleteKeyAsync] Error deleting key: {ex.Message}");
                return false;
            }
        }

        protected abstract UniTask PlatformSaveAsync(string key, object transformData, CancellationToken cancellationToken);
        protected abstract UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken);
        protected abstract UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken);
        protected abstract UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken);
    }
}