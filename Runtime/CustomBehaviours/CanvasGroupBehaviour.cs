using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class CanvasGroupBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public CanvasGroup CanvasGroup => _canvasGroup = _canvasGroup ? _canvasGroup : GetComponent<CanvasGroup>();
        private CanvasGroup _canvasGroup;
    }
}