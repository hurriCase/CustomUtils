using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class AspectRatioBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public AspectRatioFitter AspectRatioFitter => _aspectRatioFitter =
            _aspectRatioFitter ? _aspectRatioFitter : GetComponent<AspectRatioFitter>();
        private AspectRatioFitter _aspectRatioFitter;
    }
}