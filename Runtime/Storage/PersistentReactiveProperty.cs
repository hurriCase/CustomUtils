using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Storage
{
    public sealed class PersistentReactiveProperty<T> : IDisposable, IPersistentProperty
    {
        private readonly string _key;
        private readonly IStorageProvider _provider;
        private readonly IDisposable _subscription;
        private bool _savingEnabled = true;

        private readonly ReactiveProperty<T> _property;

        public T Value
        {
            get => _property.Value;
            set => _property.Value = value;
        }

        public PersistentReactiveProperty(string key, IStorageProvider serviceProvider, T defaultValue = default)
        {
            _key = key;
            _provider = serviceProvider;
            _property = new ReactiveProperty<T>(defaultValue);

            _subscription = _property.Subscribe(_ =>
            {
                if (_savingEnabled)
                    SaveAsync().Forget();
            });

            Initialize().Forget();
        }

        public IDisposable Subscribe(Action<T> onNext) => _property.Subscribe(onNext);

        private async UniTask Initialize()
        {
            try
            {
                _savingEnabled = false;

                var loaded = await _provider.LoadAsync<T>(_key);
                if (loaded != null && EqualityComparer<T>.Default.Equals(loaded, default) is false)
                    _property.Value = loaded;
            }
            catch (Exception ex)
            {
                Debug.LogError("[PersistentReactiveProperty::Initialize] Value loading failed with error: " +
                               $"{ex.Message}");
            }
            finally
            {
                _savingEnabled = true;
            }
        }

        public async UniTask SaveAsync()
        {
            await _provider.SaveAsync(_key, _property.Value);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _property.Dispose();
        }
    }
}