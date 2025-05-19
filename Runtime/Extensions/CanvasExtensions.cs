using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace CustomUtils.Runtime.Extensions
{
    [UsedImplicitly]
    public static class CanvasExtensions
    {
        /// <summary>
        /// Hides a canvas group by setting it to transparent and disabling interaction.
        /// </summary>
        /// <param name="canvasGroup">The canvas group to hide.</param>
        /// <remarks>
        /// Sets alpha to 0 and disables both interactable and blocksRaycasts properties.
        /// </remarks>
        [UsedImplicitly]
        public static void Hive(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Shows a canvas group by making it fully visible and enabling interaction.
        /// </summary>
        /// <param name="canvasGroup">The canvas group to show.</param>
        /// <remarks>
        /// Sets alpha to 1 and enables both interactable and blocksRaycasts properties.
        /// </remarks>
        [UsedImplicitly]
        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}