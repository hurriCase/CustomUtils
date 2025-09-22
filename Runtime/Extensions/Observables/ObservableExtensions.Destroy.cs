﻿using System;
using JetBrains.Annotations;
using R3;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions.Observables
{
    /// <summary>
    /// Extension methods for Observable subscription with automatic disposal on destruction.
    /// </summary>
    public static partial class ObservableExtensions
    {
        /// <summary>
        /// Subscribes to observable and automatically disposes on MonoBehaviour destruction.
        /// </summary>
        /// <typeparam name="TSelf">MonoBehaviour type.</typeparam>
        /// <typeparam name="T">Observable value type.</typeparam>
        /// <param name="observable">Observable to subscribe to.</param>
        /// <param name="self">MonoBehaviour instance for disposal registration.</param>
        /// <param name="onNext">Action called with MonoBehaviour instance.</param>
        [UsedImplicitly]
        public static void SubscribeUntilDestroy<TSelf, T>(
            this Observable<T> observable,
            TSelf self,
            Action<TSelf> onNext)
            where TSelf : MonoBehaviour
        {
            observable.Subscribe((self, onNext), static (_, tuple) => tuple.onNext(tuple.self))
                .RegisterTo(self.destroyCancellationToken);
        }

        /// <summary>
        /// Subscribes to observable and automatically disposes on MonoBehaviour destruction.
        /// </summary>
        /// <typeparam name="TSelf">MonoBehaviour type.</typeparam>
        /// <typeparam name="T">Observable value type.</typeparam>
        /// <param name="observable">Observable to subscribe to.</param>
        /// <param name="self">MonoBehaviour instance for disposal registration.</param>
        /// <param name="onNext">Action called with observable value and MonoBehaviour instance.</param>
        [UsedImplicitly]
        public static void SubscribeUntilDestroy<TSelf, T>(
            this Observable<T> observable,
            TSelf self,
            Action<T, TSelf> onNext)
            where TSelf : MonoBehaviour
        {
            observable.Subscribe((self, onNext), static (value, tuple) => tuple.onNext(value, tuple.self))
                .RegisterTo(self.destroyCancellationToken);
        }

        /// <summary>
        /// Subscribes to observable and automatically disposes on MonoBehaviour destruction.
        /// </summary>
        /// <typeparam name="TSelf">MonoBehaviour type.</typeparam>
        /// <typeparam name="T">Observable value type.</typeparam>
        /// <typeparam name="TTuple">Additional data type.</typeparam>
        /// <param name="observable">Observable to subscribe to.</param>
        /// <param name="self">MonoBehaviour instance for disposal registration.</param>
        /// <param name="tuple">Additional data passed to the action.</param>
        /// <param name="onNext">Action called with additional data and MonoBehaviour instance.</param>
        [UsedImplicitly]
        public static void SubscribeUntilDestroy<TSelf, T, TTuple>(
            this Observable<T> observable,
            TSelf self,
            TTuple tuple,
            Action<TTuple, TSelf> onNext)
            where TSelf : MonoBehaviour
        {
            observable.Subscribe((self, tuple, onNext),
                    static (_, state) => state.onNext(state.tuple, state.self))
                .RegisterTo(self.destroyCancellationToken);
        }

        /// <summary>
        /// Subscribes to observable and automatically disposes on MonoBehaviour destruction.
        /// </summary>
        /// <typeparam name="TSelf">MonoBehaviour type.</typeparam>
        /// <typeparam name="T">Observable value type.</typeparam>
        /// <typeparam name="TTuple">Additional data type.</typeparam>
        /// <param name="observable">Observable to subscribe to.</param>
        /// <param name="self">MonoBehaviour instance for disposal registration.</param>
        /// <param name="tuple">Additional data passed to the action.</param>
        /// <param name="onNext">Action called with observable value, MonoBehaviour instance, and additional data.</param>
        [UsedImplicitly]
        public static void SubscribeUntilDestroy<TSelf, T, TTuple>(
            this Observable<T> observable,
            TSelf self,
            TTuple tuple,
            Action<T, TSelf, TTuple> onNext)
            where TSelf : MonoBehaviour
        {
            observable.Subscribe((self, tuple, onNext),
                    static (value, state) => state.onNext(value, state.self, state.tuple))
                .RegisterTo(self.destroyCancellationToken);
        }
    }
}