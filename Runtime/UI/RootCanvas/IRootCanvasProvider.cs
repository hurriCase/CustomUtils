using JetBrains.Annotations;
using R3;

namespace CustomUtils.Runtime.UI.RootCanvas
{
    /// <summary>
    /// Provides access to canvas scale factor and reactive updates for UI scaling operations.
    /// </summary>
    [UsedImplicitly]
    public interface IRootCanvasProvider
    {
        /// <summary>
        /// Gets the current scale factor of the root canvas.
        /// Returns 1.0 if no canvas is assigned.
        /// </summary>
        [UsedImplicitly]
        float ScaleFactor { get; }

        /// <summary>
        /// Gets an observable that emits the canvas scale factor whenever it changes.
        /// Returns a constant observable with value 1.0 if no canvas is assigned.
        /// </summary>
        [UsedImplicitly]
        Observable<float> CanvasScaleFactorObservable { get; }
    }
}