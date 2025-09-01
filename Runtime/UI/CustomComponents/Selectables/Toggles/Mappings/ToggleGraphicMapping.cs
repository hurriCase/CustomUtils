using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables.Toggles.Mappings
{
    [Serializable]
    [UsedImplicitly]
    public struct ToggleGraphicMapping
    {
        [SerializeField] private Selectable.Transition _transitionType;

        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private ToggleColorMapping _colorMapping;

        [SerializeField] private Image _targetImage;
        [SerializeField] private ToggleSpriteMapping _spriteMapping;

        [UsedImplicitly]
        public void ApplyState(ToggleStateType state)
        {
            switch (_transitionType)
            {
                case Selectable.Transition.ColorTint:
                    ApplyColorTransition(state);
                    break;

                case Selectable.Transition.SpriteSwap:
                    ApplySpriteTransition(state);
                    break;
            }
        }

        private void ApplyColorTransition(ToggleStateType state)
        {
            if (!_targetGraphic || !_colorMapping)
                return;

            _targetGraphic.color = _colorMapping.GetColorForState(state);
        }

        private void ApplySpriteTransition(ToggleStateType state)
        {
            if (!_targetImage || !_spriteMapping)
                return;

            _targetImage.overrideSprite = _spriteMapping.GetSpriteForState(state);
        }
    }
}