using System;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    internal abstract class ColorModifierBase : MonoBehaviour, IDisposable
    {
        internal abstract void UpdateColor(string colorName);
        public abstract void Dispose();
    }
}