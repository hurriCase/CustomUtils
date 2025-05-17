using System;
using CustomUtils.Runtime.AssetLoader;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable StaticMemberInGenericType
namespace CustomUtils.Runtime.CustomTypes.Singletons
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for MonoBehaviours that follow the Singleton pattern and persist between scenes.
    /// Can be instantiated from a prefab in Resources folder or created dynamically.
    /// </summary>
    /// <typeparam name="T">The type of MonoBehaviour to make a persistent singleton.</typeparam>
    public abstract class PersistentSingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        [UsedImplicitly] public static T Instance
        {
            get
            {
                if (_created)
                    return _instance;

                _instance = CreateInstance();
                _created = true;

                return _instance;
            }
        }

        public static event Action OnDestroyed;

        private static bool _created;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticVariables()
        {
            _instance = null;
            OnDestroyed = null;
            _created = false;
        }
#endif
        protected virtual void Awake()
        {
            if (_instance is null)
            {
                if (_created is false)
                    _instance = CreateInstance();

                DontDestroyOnLoad(gameObject);
                return;
            }

            if (_instance != this)
                Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance != this)
                return;

            _instance = null;

            OnDestroyed?.Invoke();
        }

        private static T CreateInstance()
        {
            var type = typeof(T);
            var instances = FindObjectsByType<T>(FindObjectsSortMode.None);

            if (instances.Length > 0)
            {
                if (instances.Length <= 1)
                    return instances[0];

                Debug.LogWarning($"There is more than one instance of Singleton of type '{type}'." +
                                 " Keeping the first one. Destroying the others.");
                for (var i = 1; i < instances.Length; i++)
                    Destroy(instances[i].gameObject);

                return instances[0];
            }

            var prefabName = type.Name;

            if (ResourceLoader<T>.TryLoad(out var prefab))
                prefabName = prefab.name;

            var gameObject = prefab ? Instantiate(prefab.gameObject) : new GameObject(prefabName);

            return _instance ??= gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
    }
}