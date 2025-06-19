using CustomUtils.Runtime.UI.RatioLayout;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.CustomBehaviours
{
    [UsedImplicitly]
    public class RatioLayoutBehaviour : MonoBehaviour
    {
        [UsedImplicitly]
        public RatioLayoutElement RatioLayoutElement => _ratioLayoutElement =
            _ratioLayoutElement ? _ratioLayoutElement : GetComponent<RatioLayoutElement>();
        private RatioLayoutElement _ratioLayoutElement;
    }
}