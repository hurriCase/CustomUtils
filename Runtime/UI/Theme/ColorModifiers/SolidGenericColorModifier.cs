using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.Base;
using CustomUtils.Runtime.UI.Theme.Databases;
using CustomUtils.Runtime.UI.Theme.Databases.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [ColorModifier(ColorType.Solid)]
    internal sealed class SolidGenericColorModifier : GenericColorModifierBase<Color>
    {
        [SerializeField, InspectorReadOnly] private Graphic _graphic;

        internal override IThemeDatabase<Color> ThemeDatabase => SolidColorDatabase.Instance;

        protected override void OnEnable()
        {
            base.OnEnable();

            _graphic = _graphic.AsNullable() ?? GetComponent<Graphic>();
        }

        protected override void OnApplyColor(Color gradient)
        {
            _graphic.color = gradient;
        }

        private void OnDestroy()
        {
            _graphic.color = Color.white;
        }
    }
}