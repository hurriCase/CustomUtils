using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.Base;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme
{
    /// <inheritdoc />
    /// <summary>
    /// Component responsible for applying theme colors to UI graphics.
    /// Automatically creates and manages appropriate color modifiers based on the color type.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    [UsedImplicitly]
    public sealed class ThemeComponent : MonoBehaviour
    {
        [SerializeField] private ColorData _colorData;

        private ColorModifierBase _currentColorModifier;

        private ColorData _previousColorData;

        /// <summary>
        /// Updates the color data for this theme component and applies the corresponding color modifier.
        /// </summary>
        /// <param name="colorData">The new color data to apply.</param>
        [UsedImplicitly]
        public void UpdateColorData(ColorData colorData)
        {
            _colorData = colorData;
            UpdateModifier(colorData);
        }

        private void UpdateModifier(ColorData colorData)
        {
            if (_previousColorData != colorData || !_currentColorModifier)
            {
                CreateModifier(colorData.ColorType);
                _previousColorData = colorData;
            }

            _currentColorModifier.AsNullable()?.UpdateColor(colorData.ColorName);
        }

        private void CreateModifier(ColorType colorType)
        {
            _currentColorModifier.AsNullable()?.Dispose();
            _currentColorModifier.AsNullable()?.Destroy();
            _currentColorModifier = ColorModifierFactory.CreateModifier(colorType, gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(this))
                return;

            // We can't destroy an object during OnValidate
            EditorApplication.delayCall += () =>
            {
                if (this && _previousColorData != _colorData)
                    UpdateModifier(_colorData);
            };
        }
#endif
    }
}