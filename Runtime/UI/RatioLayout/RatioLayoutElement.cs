using CustomUtils.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.RatioLayout
{
    [AddComponentMenu("Layout/Ratio Layout Element")]
    [RequireComponent(typeof(RectTransform))]
    internal sealed class RatioLayoutElement : MonoBehaviour
    {
        [field: SerializeField] internal float Ratio { get; private set; }

        private void OnValidate()
        {
            NotifyLayoutChanged();
        }

        private void NotifyLayoutChanged()
        {
            if (this.TryGetComponentInParent<RatioLayoutGroup>(out var layoutGroup))
                LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.transform as RectTransform);
        }
    }
}