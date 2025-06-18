using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [Serializable]
    internal struct PixelPerUnitData
    {
        [field: SerializeField] internal string Name { get; private set; }
        [field: SerializeField] internal float CornerRatio { get; private set; }
    }
}