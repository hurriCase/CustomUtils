using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class RectTransformBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        protected RectTransform RectTransform =>
            _rectTransform = _rectTransform ? _rectTransform : GetComponent<RectTransform>();
        private RectTransform _rectTransform;
    }
}