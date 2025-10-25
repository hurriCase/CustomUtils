using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Api.Interfaces
{
    /// <summary>
    /// Checks API endpoint availability by sending test requests.
    /// </summary>
    [PublicAPI]
    public interface IApiAvailabilityChecker
    {
        /// <summary>
        /// Checks if the specified API endpoint is available.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <param name="contentCode">The expected HTTP response code for availability.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if the endpoint is available; otherwise, false.</returns>
        UniTask<bool> IsAvailable(string url, long contentCode, CancellationToken token);
    }
}