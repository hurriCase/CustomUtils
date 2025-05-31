using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Storage
{
    [UsedImplicitly]
    public sealed class PersistentReactiveProperty<TProperty> : IDisposable, IPersistentProperty
    {
        private readonly string _key;
        private readonly IStorageProvider _provider;
        private readonly IDisposable _subscription;
        private bool _savingEnabled = true;

        private readonly ReactiveProperty<TProperty> _property;

        /// <summary>
        /// Gets or sets the current value of the property
        /// </summary>
        [UsedImplicitly]
        public TProperty Value
        {
            get => _property.Value;
            set => _property.Value = value;
        }

        /// <summary>
        /// Creates a new persistent reactive property with automatic provider resolution
        /// </summary>
        /// <param name="key">Unique key for storing the value</param>
        /// <param name="defaultValue">Default value if no saved value exists</param>
        [UsedImplicitly]
        public PersistentReactiveProperty(string key, TProperty defaultValue = default)
        {
            _key = key;

            _provider = ServiceProvider.Provider;
            _property = new ReactiveProperty<TProperty>(defaultValue);

            _subscription = _property.Subscribe(this, static (_, state) =>
            {
                if (state._savingEnabled)
                    state.SaveAsync().Forget();
            });

            Initialize().Forget();
        }

        /// <summary>
        /// Subscribes to value changes
        /// </summary>
        /// <param name="target">Target object to pass to the callback</param>
        /// <param name="onNext">Action to execute when value changes</param>
        /// <returns>Disposable subscription</returns>
        [UsedImplicitly]
        public IDisposable Subscribe<TTarget>(TTarget target, Action<TTarget, TProperty> onNext) where TTarget : class
            => _property.Subscribe(
                (target, onNext),
                static (value, tuple) => tuple.onNext(tuple.target, value));

        /// <summary>
        /// Manually saves the current value to storage
        /// </summary>
        /// <returns>Task representing the save operation</returns>
        [UsedImplicitly]
        public async UniTask SaveAsync()
        {
            try
            {
                await _provider.SaveAsync(_key, _property.Value);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PersistentReactiveProperty::SaveAsync] Failed to save key '{_key}': {ex.Message}");
            }
        }

        private async UniTask Initialize()
        {
            try
            {
                _savingEnabled = false;

                var loaded = await _provider.LoadAsync<TProperty>(_key);
                if (loaded != null && EqualityComparer<TProperty>.Default.Equals(loaded, default) is false)
                    _property.Value = loaded;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PersistentReactiveProperty::Initialize] Failed to load key '{_key}': {ex.Message}");
            }
            finally
            {
                _savingEnabled = true;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Disposes the property and stops persistence
        /// </summary>
        [UsedImplicitly]
        public void Dispose()
        {
            _subscription?.Dispose();
            _property.Dispose();
        }
    }
}