using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [Serializable]
    internal struct ThemeGraphicMapping
    {
        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private SelectableColorMapping _colorMapping;

        internal void ApplyColor(SelectableStateType state)
        {
            if (!_colorMapping || !_targetGraphic)
                return;

            _targetGraphic.color = _colorMapping.GetColorForState(state);
        }
    }
}