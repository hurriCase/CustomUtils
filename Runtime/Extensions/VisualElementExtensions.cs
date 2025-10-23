using UnityEngine.UIElements;

namespace CustomUtils.Runtime.Extensions
{
    internal static class VisualElementExtensions
    {
        internal static void SetActive(this VisualElement visualElement, bool isActive)
        {
            visualElement.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}