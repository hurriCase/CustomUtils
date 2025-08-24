using System;
using System.Threading;
using CustomUtils.Runtime.AddressableSystem;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CustomUtils.Runtime.Scenes
{
    [UsedImplicitly]
    public sealed class SceneLoader : ISceneLoader
    {
        private static SceneInstance _sceneInstance;

        public async UniTask LoadSceneAsync(
            string sceneAddress,
            CancellationToken token,
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            try
            {
#if ADDRESSABLES_LOG_ALL
                using var stopWatchScope = AddressablesLogger.LogWithTimePast("[SceneLoader::LoadSceneAsync]");
#endif

                AddressablesLogger.Log($"[SceneLoader::LoadSceneAsync] Start loading scene: {sceneAddress}");

                var currentScene = await Addressables.LoadSceneAsync(sceneAddress, loadMode).WithCancellation(token);

                AddressablesLogger.Log($"[SceneLoader::LoadSceneAsync] Scene loaded successfully: {sceneAddress}");

                TryUnloadScene(_sceneInstance);

                _sceneInstance = currentScene;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneLoader::LoadSceneAsync] Failed to load scene {sceneAddress}: {ex.Message}");
                throw;
            }
        }

        public void TryUnloadScene(SceneInstance sceneInstance)
        {
            if (sceneInstance.Scene.IsValid() is false)
                return;

            AddressablesLogger.Log($"[SceneLoader::TryUnloadScene] Start unloading scene: {sceneInstance.Scene.name}");

            Addressables.UnloadSceneAsync(sceneInstance);

            AddressablesLogger.Log($"[SceneLoader::TryUnloadScene] Scene unloaded: {sceneInstance.Scene.name}");
        }
    }
}