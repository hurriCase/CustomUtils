using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    [Serializable]
    [UsedImplicitly]
    public struct ThemeGraphicMapping
    {
        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private SelectableColorMapping _colorMapping;

        [UsedImplicitly]
        public void ApplyColor(SelectableStateType state)
        {
            if (!_colorMapping || !_targetGraphic)
                return;

            _targetGraphic.color = _colorMapping.GetColorForState(state);
        }
    }
}