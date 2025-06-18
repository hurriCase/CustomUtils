﻿using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [Serializable]
    internal struct PixelPerUnitData
    {
        [field: SerializeField] internal string Name { get; private set; }
        [field: SerializeField] internal DimensionType DimensionType { get; private set; }

        [field: SerializeField] internal float ImageSize { get; private set; }
        [field: SerializeField] internal float CornerRadius { get; private set; }

        internal float CornerRatio => ImageSize / CornerRadius;
    }
}