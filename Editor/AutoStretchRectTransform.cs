#if IS_STRETCH_RECTRANSFORM
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    [InitializeOnLoad]
    public class AutoStretchRectTransform
    {
        static AutoStretchRectTransform()
        {
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            if (!component || component is not RectTransform rectTransform)
                return;

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
#endif
