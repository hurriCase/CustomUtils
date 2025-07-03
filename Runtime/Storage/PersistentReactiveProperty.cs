﻿using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Storage.Base;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Storage
{
    [UsedImplicitly]
    public sealed class PersistentReactiveProperty<TProperty> : IDisposable
    {
        private readonly ReactiveProperty<TProperty> _property;

        private readonly string _key;
        private readonly IDisposable _subscription;

        private IStorageProvider _provider;
        private bool _savingEnabled;
        private bool _loadAttempted;
        private bool _isLoading;

        /// <summary>
        /// Gets or sets the current value of the property
        /// </summary>
        [UsedImplicitly]
        public TProperty Value
        {
            get
            {
                EnsureLoaded();
                return _property.Value;
            }
            set => _property.Value = value;
        }

        /// <summary>
        /// Modifies the current value of the property
        /// </summary>
        /// <param name="input">Input value</param>
        /// <param name="modifier">Modifier function</param>
        /// <typeparam name="TInput">Type of the input value</typeparam>
        /// <returns>Modified value</returns>
        [UsedImplicitly]
        public void ModifyValue<TInput>(TInput input, Action<TInput, TProperty> modifier)
        {
            var value = Value;
            modifier(input, value);
            Value = value;
        }

        /// <summary>
        /// Modifies the current value of the property
        /// </summary>
        /// <param name="modifier">Modifier function</param>
        /// <returns>Modified value</returns>
        [UsedImplicitly]
        public void ModifyValue(Action<TProperty> modifier)
        {
            var value = Value;
            modifier(value);
            Value = value;
        }

        /// <summary>
        /// Gets the observable stream for this property
        /// </summary>
        [UsedImplicitly]
        public Observable<TProperty> AsObservable()
        {
            EnsureLoaded();
            return _property.AsObservable();
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
            _property = new ReactiveProperty<TProperty>(defaultValue);

            _subscription = _property.Subscribe(this, static (_, self) =>
            {
                if (self._savingEnabled)
                    self.SaveAsync().Forget();
            });
        }

        /// <summary>
        /// Factory method that ensures proper async initialization
        /// Recommended for Unity environments
        /// </summary>
        /// <param name="key">Unique key for storing the value</param>
        /// <param name="defaultValue">Default value if no saved value exists</param>
        /// <returns>Fully initialized PersistentReactiveProperty</returns>
        [UsedImplicitly]
        public static async UniTask<PersistentReactiveProperty<TProperty>> CreateAsync(string key,
            TProperty defaultValue = default)
        {
            var property = new PersistentReactiveProperty<TProperty>(key, defaultValue);
            await property.InitializeAsync();
            return property;
        }

        /// <summary>
        /// Subscribes to value changes
        /// </summary>
        /// <param name="target">Target object to pass to the callback</param>
        /// <param name="onNext">Action to execute when value changes</param>
        /// <returns>Disposable subscription</returns>
        [UsedImplicitly]
        public IDisposable Subscribe<TTarget>(TTarget target, Action<TProperty, TTarget> onNext)
        {
            EnsureLoaded();

            return _property.Subscribe(
                (target, onNext),
                static (property, tuple) => tuple.onNext(property, tuple.target));
        }

        /// <summary>
        /// Subscribes to value changes with a static action that ignores the value
        /// </summary>
        /// <param name="onNext">Static action to execute when value changes</param>
        /// <returns>Disposable subscription</returns>
        [UsedImplicitly]
        public IDisposable Subscribe(Action onNext)
        {
            EnsureLoaded();
            return _property.Subscribe(onNext, static (_, action) => action());
        }

        /// <summary>
        /// Manually saves the current value to storage
        /// </summary>
        /// <returns>Task representing the save operation</returns>
        [UsedImplicitly]
        public async UniTask SaveAsync()
        {
            EnsureProvider();

            if (_provider != null)
                await _provider.TrySaveAsync(_key, _property.Value);
        }

        /// <summary>
        /// Manually initializes the property (loads from storage)
        /// </summary>
        /// <returns>Task representing the initialization</returns>
        [UsedImplicitly]
        public async UniTask InitializeAsync()
        {
            if (_loadAttempted || _isLoading)
                return;

            _isLoading = true;

            try
            {
                EnsureProvider();

                if (_provider == null)
                    return;

                var loaded = await _provider.LoadAsync<TProperty>(_key);
                if (loaded != null && EqualityComparer<TProperty>.Default.Equals(loaded, default) is false)
                    _property.Value = loaded;
            }
            catch (Exception ex)
            {
                Debug.LogError("[PersistentReactiveProperty::InitializeAsync] " +
                               $"Failed to load key '{_key}': {ex.Message}");
            }
            finally
            {
                _loadAttempted = true;
                _savingEnabled = true;
                _isLoading = false;
            }
        }

        private void EnsureLoaded()
        {
            if (_loadAttempted is false)
                InitializeAsync().Forget();
        }

        private void EnsureProvider()
        {
            if (_provider != null)
                return;

            try
            {
                _provider = ServiceProvider.Provider;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PersistentReactiveProperty::EnsureProvider] Failed to get provider: {ex.Message}");
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