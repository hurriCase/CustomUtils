using R3;
using UnityEngine;

namespace CustomUtils.Runtime.UI.RootCanvas
{
    [CreateAssetMenu(fileName = nameof(RootCanvasProvider), menuName = nameof(RootCanvasProvider))]
    public sealed class RootCanvasProvider : ScriptableObject, IRootCanvasProvider
    {
        [field: SerializeField] public Canvas RootCanvas { get; private set; }

        public float ScaleFactor => RootCanvas ? RootCanvas.scaleFactor : 1f;

        public Observable<float> CanvasScaleFactorObservable =>
            RootCanvas
                ? Observable.EveryValueChanged(this, static self => self.RootCanvas.scaleFactor)
                : Observable.Return(1f);
    }
}