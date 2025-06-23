using System;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.CustomTypes.Singletons
{
    public abstract class Singleton<T> where T : class
    {
        private static T _instance;

#if UNITY_EDITOR
        static Singleton()
        {
            SingletonResetter.RegisterResetAction(() =>
            {
                _instance = null;
            });
        }
#endif

        [UsedImplicitly]
        public static T Instance => _instance = _instance ?? (_instance = Activator.CreateInstance(typeof(T)) as T);
    }
}