﻿using System;
using System.Collections.Generic;
using System.Threading;
using CustomUtils.Runtime.Storage.Base;
using CustomUtils.Runtime.Storage.DataTransformers;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Storage.Providers
{
    [UsedImplicitly]
    internal sealed class FirebaseStorageProvider : BaseStorageProvider
    {
        private const long MaxDownloadSize = 5 * 1024 * 1024;

        private readonly FirebaseStorage _firebaseStorage;
        private readonly string _storagePath;
        private readonly Dictionary<string, byte[]> _memoryCache = new();

        internal FirebaseStorageProvider(string userId = null) : base(new IdentityDataTransformer())
        {
            _firebaseStorage = FirebaseStorage.DefaultInstance;

            userId = string.IsNullOrEmpty(userId) ? SystemInfo.deviceUniqueIdentifier : userId;

            _storagePath = $"users/{userId}";

            Debug.Log($"[FirebaseStorageProvider::FirebaseStorageProvider] Initialized with user ID: {userId}");
        }

        private StorageReference GetFileReference(string key)
            => _firebaseStorage.GetReference($"{_storagePath}/{key}");

        protected override async UniTask PlatformSaveAsync(string key, object transformData,
            CancellationToken cancellationToken)
        {
            if (transformData is not byte[] byteData)
                return;

            try
            {
                _memoryCache[key] = byteData;

                var reference = GetFileReference(key);

                await reference.PutBytesAsync(byteData, cancelToken: cancellationToken)
                    .AsUniTask().AttachExternalCancellation(cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.LogError("[FirebaseStorageProvider::PlatformSaveAsync] " +
                               $"Error saving data for key {key}: {ex.Message}");
            }
        }

        protected override async UniTask<object> PlatformLoadAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var cachedBytes))
                    return cachedBytes;

                var reference = GetFileReference(key);

                if (await PlatformHasKeyAsync(key, cancellationToken) is false)
                    return null;

                var downloadTask = await reference.GetBytesAsync(MaxDownloadSize)
                    .AsUniTask().AttachExternalCancellation(cancellationToken);

                if (downloadTask == null || downloadTask.Length == 0)
                    return null;

                _memoryCache[key] = downloadTask;

                return downloadTask;
            }
            catch (Exception ex)
            {
                Debug.LogError("[FirebaseStorageProvider::PlatformLoadAsync] " +
                               $"Error loading data for key {key}: {ex.Message}");
                return null;
            }
        }

        protected override async UniTask<bool> PlatformHasKeyAsync(string key, CancellationToken cancellationToken)
        {
            if (_memoryCache.ContainsKey(key))
                return true;

            try
            {
                var reference = GetFileReference(key);

                var result = await reference.GetMetadataAsync()
                    .ContinueWithOnMainThread(static task => task.IsFaulted is false && task.IsCanceled is false)
                    .AsUniTask()
                    .AttachExternalCancellation(cancellationToken);

                return result;
            }
            catch
            {
                return false;
            }
        }

        protected override async UniTask PlatformDeleteKeyAsync(string key, CancellationToken cancellationToken)
        {
            try
            {
                _memoryCache.Remove(key);

                var reference = GetFileReference(key);

                if (await PlatformHasKeyAsync(key, cancellationToken) is false)
                    return;

                await reference.DeleteAsync().AsUniTask().AttachExternalCancellation(cancellationToken);
            }
            catch (Exception ex)
            {
                Debug.LogError("[FirebaseStorageProvider::PlatformDeleteKeyAsync] " +
                               $"Error deleting key {key}: {ex.Message}");
            }
        }

        protected override UniTask<bool> PlatformTryDeleteAllAsync(CancellationToken cancellationToken) =>
            UniTask.FromResult(false);
    }
}