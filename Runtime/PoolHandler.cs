using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime
{
    public class PoolHandler<TPoolEntity> where TPoolEntity : Object
    {
        private TPoolEntity _prefab;
        private Transform _parent;
        private Action<TPoolEntity> _onCreateCallback;
        private Action<TPoolEntity> _onGetCallback;
        private Action<TPoolEntity> _onReleaseCallback;
        private Action<TPoolEntity> _onDestroyCallback;

        private IObjectPool<TPoolEntity> _pool;

        public void Init(
            TPoolEntity prefab,
            int defaultPoolSize = 10,
            int maxPoolSize = 100,
            Action<TPoolEntity> onCreateCallback = null,
            Action<TPoolEntity> onGetCallback = null,
            Action<TPoolEntity> onReleaseCallback = null,
            Action<TPoolEntity> onDestroyCallback = null,
            Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            _onCreateCallback = onCreateCallback;
            _onGetCallback = onGetCallback;
            _onReleaseCallback = onReleaseCallback;
            _onDestroyCallback = onDestroyCallback;

            _pool = new ObjectPool<TPoolEntity>(
                CreateEntity,
                OnGet,
                OnRelease,
                OnDestroy,
                false,
                defaultPoolSize,
                maxPoolSize);
        }

        protected virtual TPoolEntity CreateEntity()
        {
            var entity = Object.Instantiate(_prefab, _parent);
            _onCreateCallback?.Invoke(entity);
            SetActive(entity, false);
            return entity;
        }

        protected virtual void OnGet(TPoolEntity entity)
        {
            _onGetCallback?.Invoke(entity);
            SetActive(entity, true);
        }

        protected virtual void OnRelease(TPoolEntity entity)
        {
            _onReleaseCallback?.Invoke(entity);
            SetActive(entity, false);
        }

        protected virtual void OnDestroy(TPoolEntity entity)
        {
            _onDestroyCallback?.Invoke(entity);
            Object.Destroy(entity);
        }

        public TPoolEntity Get() => _pool.Get();

        public void Release(TPoolEntity element) => _pool.Release(element);

        public void Clear() => _pool.Clear();

        private void SetActive(TPoolEntity entity, bool active)
        {
            if (!entity)
            {
                Debug.LogWarning("Attempted to set active state on a destroyed object");
                return;
            }

            switch (entity)
            {
                case GameObject gameObject:
                    gameObject.SetActive(active);
                    break;

                case Component component:
                    component.gameObject.SetActive(active);
                    break;

                default:
                    Debug.LogWarning($"Cannot set active state on {entity.GetType().Name}. " +
                                     "PoolHandler only supports GameObject and Component types.");
                    break;
            }
        }
    }
}