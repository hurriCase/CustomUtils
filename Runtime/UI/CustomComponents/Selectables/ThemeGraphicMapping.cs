using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [Serializable]
    public struct ThemeGraphicMapping
    {
        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private SelectableColorMapping _colorMapping;

        public void ApplyColor(SelectableStateType state)
        {
            if (!_colorMapping || !_targetGraphic)
                return;

            _targetGraphic.color = _colorMapping.GetColorForState(state);
        }
    }
}