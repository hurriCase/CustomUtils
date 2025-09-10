using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.CustomCorners
{
    [Serializable]
    public struct CornerRadiiData
    {
        [field: SerializeField] public float LeftTop { get; set; }
        [field: SerializeField] public float RightTop { get; set; }
        [field: SerializeField] public float RightBottom { get; set; }
        [field: SerializeField] public float LeftBottom { get; set; }
    }
}