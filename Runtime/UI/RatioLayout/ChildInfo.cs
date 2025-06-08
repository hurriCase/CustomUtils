using UnityEngine;

namespace CustomUtils.Runtime.UI.RatioLayout
{
    internal struct ChildInfo
    {
        internal RectTransform RectTransform { get; set; }
        internal RatioLayoutElement RatioElement { get; set; }
        internal bool IsRatio { get; set; }
    }
}