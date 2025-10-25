using System;
using System.Threading;
using CustomUtils.Runtime.Api.Interfaces;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace CustomUtils.Runtime.Api
{
    [PublicAPI]
    public sealed class ApiAvailabilityChecker : IApiAvailabilityChecker
    {
        private const int TimeoutSeconds = 5;

        public async UniTask<bool> IsAvailable(string url, long contentCode, CancellationToken token)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            try
            {
                using var request = UnityWebRequest.Get(url);
                request.timeout = TimeoutSeconds;

                await request.SendWebRequest().WithCancellation(token);

                return request.result == UnityWebRequest.Result.Success &&
                       request.responseCode == contentCode;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError("[ApiAvailabilityChecker::IsAvailable] " +
                               $"Api Service isn't available, for url: {url}, with exception: {ex.Message}");
                return false;
            }
        }
    }
}