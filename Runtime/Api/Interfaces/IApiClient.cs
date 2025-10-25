#nullable enable
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace CustomUtils.Runtime.Api.Interfaces
{
    /// <summary>
    /// Provides HTTP client functionality for API communication.
    /// </summary>
    [PublicAPI]
    public interface IApiClient
    {
        /// <summary>
        /// Sends a POST request with the specified request data.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The request data.</param>
        /// <param name="url">The target URL.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The response data, or null if the request fails.</returns>
        UniTask<TResponse?> PostAsync<TRequest, TResponse>(TRequest request, string url, CancellationToken token)
            where TRequest : class
            where TResponse : class;

        /// <summary>
        /// Sends a POST request with custom headers.
        /// </summary>
        /// <typeparam name="TSource">The source type for header configuration.</typeparam>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="source">The source object for header configuration.</param>
        /// <param name="request">The request data.</param>
        /// <param name="url">The target URL.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="additionalHeaders">Callback to configure additional headers.</param>
        /// <returns>The response data, or null if the request fails.</returns>
        UniTask<TResponse?> PostAsync<TSource, TRequest, TResponse>(
            TSource source,
            TRequest request,
            string url,
            CancellationToken token,
            Action<TSource, UnityWebRequest> additionalHeaders)
            where TRequest : class
            where TResponse : class;
    }
}