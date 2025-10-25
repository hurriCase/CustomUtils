using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;

namespace CustomUtils.Runtime.Api.Interfaces
{
    /// <summary>
    /// Represents an API service with availability monitoring.
    /// </summary>
    [PublicAPI]
    public interface IApiService
    {
        /// <summary>
        /// Gets the current availability status of the API.
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsAvailable { get; }

        /// <summary>
        /// Updates the availability status by checking the API endpoint.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        UniTask UpdateAvailable(CancellationToken token);
    }
}