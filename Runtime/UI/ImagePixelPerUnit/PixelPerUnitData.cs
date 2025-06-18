using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [Serializable]
    internal struct PixelPerUnitData
    {
        [field: SerializeField] internal string Name { get; private set; }
        [field: SerializeField] internal DimensionType DimensionType { get; set; }

        [SerializeField] internal float _imageSize;
        [SerializeField] internal float _cornerRadius;

        internal float CornerRatio => _imageSize / _cornerRadius;
    }
}