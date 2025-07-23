﻿using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.CustomComponents.Selectables
{
    public sealed class SelectableButton : Button
    {
        [field: SerializeField] public SelectableColorMapping SelectableColorMapping { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (!SelectableColorMapping || transition != Transition.ColorTint)
                return;

            colors = SelectableColorMapping.GetThemeBlockColors(colors);
        }
    }
}