using System;
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

        internal const string NoneOption = "None";

        internal bool IsCorrectData => Name != NoneOption && ImageSize > 0f && CornerRadius > 0f &&
                                       DimensionType != DimensionType.None;

        internal float CornerRatio => ImageSize / CornerRadius;

        internal static PixelPerUnitData None => new()
        {
            Name = NoneOption,
            DimensionType = DimensionType.None,
            ImageSize = 0f,
            CornerRadius = 0f
        };
    }
}