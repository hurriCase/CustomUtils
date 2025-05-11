using System;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Serializable]
    internal class Sheet
    {
        [field: SerializeField] internal string Name { get; set; }
        [field: SerializeField] internal long Id { get; set; }
        [field: SerializeField] internal TextAsset TextAsset { get; set; }
    }
}