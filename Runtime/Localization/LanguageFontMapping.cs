using System;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Serializable]
    public class LanguageFontMapping
    {
        [field: SerializeField] public string Language { get; private set; }
        [field: SerializeField] public TMP_FontAsset Font { get; private set; }
    }
}