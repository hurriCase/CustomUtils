using System.Reflection;
using CustomUtils.Runtime.AssetLoader;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes.Singletons
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for singleton ScriptableObjects that are automatically loaded or created when accessed.
    /// </summary>
    /// <typeparam name="T">The type of ScriptableObject to create as a singleton.</typeparam>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        /// <summary>
        /// Gets the singleton instance of the ScriptableObject.
        /// If the instance doesn't exist, it will be loaded from resources or created if necessary.
        /// The location is determined by the ResourceAttribute applied to the class.
        /// </summary>
        [UsedImplicitly] public static T Instance
        {
            get
            {
                if (_instance)
                    return _instance;

#if UNITY_EDITOR
                var attribute = typeof(T).GetCustomAttribute<ResourceAttribute>();
                if (attribute == null)
                {
                    Debug.LogWarning(
                        $"[SingletonScriptableObject::Instance] {typeof(T).Name} missing ResourceAttribute");
                    return null;
                }

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (attribute.IsEditorResource)
                    _instance = ScriptableObjectLoader<T>.LoadEditorResource();
                else
#endif
                    _instance = ScriptableObjectLoader<T>.Load();

#if UNITY_EDITOR
                if (!_instance)
                    _instance = ScriptableObjectLoader<T>.CreateAndSaveAsset();
#endif

                return _instance;
            }
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticVariables()
        {
            _instance = null;

            ScriptableObjectLoader<T>.RemoveFromCache();
        }
#endif
    }
}