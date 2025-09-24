using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers
{
    internal abstract class ColorModifierBase : MonoBehaviour
    {
        [field: SerializeField] protected string colorName;

        [SerializeField, HideInInspector] private Graphic _graphic;

        protected Graphic Graphic => _graphic ? _graphic : _graphic = GetComponent<Graphic>();
        internal abstract List<string> GetColorNames();
        internal abstract void ApplyColor();

        internal void UpdateColor(string newName)
        {
            colorName = newName;
            ApplyColor();
        }

        internal static string GetColorNameProperty => nameof(colorName);
    }
}