using System.Collections.Generic;
using UnityEngine;

namespace CustomUtils.Runtime.AssetLoader.DontDestroyLoader
{
    internal sealed class AutoPersistent : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void MakePersistent()
        {
            if (ResourceLoader<GameObject>.TryLoadAll(AssetLoaderConfig.Instance.DontDestroyPath,
                    out var dontDestroyObjects) is false)
                return;

            var instantiatedObjects = new HashSet<string>();

            foreach (var dontDestroy in dontDestroyObjects)
            {
                if (dontDestroy.TryGetComponent<DontDestroyOnLoadComponent>(out _) is false)
                    continue;

                if (instantiatedObjects.Contains(dontDestroy.name))
                    continue;

                var instance = Instantiate(dontDestroy);
                instance.name = dontDestroy.name;
                DontDestroyOnLoad(instance);
                instantiatedObjects.Add(dontDestroy.name);
            }
        }
    }
}