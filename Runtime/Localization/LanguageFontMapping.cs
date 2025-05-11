using System;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Serializable]
    public class LanguageFontMapping
    {
        [SerializeField] public string Language;
        [SerializeField] public TMP_FontAsset Font;
    }
}