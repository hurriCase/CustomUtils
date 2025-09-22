using System.Collections.Generic;
using CustomUtils.Runtime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers
{
    internal abstract class ColorModifierBase : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Graphic _graphic;
        [field: SerializeField, ThemeColorName] internal string ColorName { get; private set; }

        protected Graphic Graphic => _graphic ? _graphic : _graphic = GetComponent<Graphic>();
        internal abstract void ApplyColor();
        internal abstract List<string> GetColorNames();

        internal void UpdateColor(string colorName)
        {
            ColorName = colorName;
            ApplyColor();
        }
    }
}