using System;
using AYellowpaper.SerializedCollections;
using CustomUtils.Runtime.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Serializable]
    public struct LocalizationKey
    {
        [field: SerializeField] public string GUID { get; private set; }

        [field: SerializeField, InspectorReadOnly]
        internal SerializedDictionary<SystemLanguage, string> Translations { get; private set; }

        public bool IsValid => string.IsNullOrEmpty(GUID) is false;
    }
}