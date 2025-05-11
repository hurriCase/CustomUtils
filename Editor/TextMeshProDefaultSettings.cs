using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    [InitializeOnLoad]
    public class TextMeshProDefaultSettings
    {
        static TextMeshProDefaultSettings()
        {
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            if (component is not TextMeshProUGUI textMeshProUGUI)
                return;

            textMeshProUGUI.enableAutoSizing = true;
            textMeshProUGUI.fontSizeMin = 0;
            textMeshProUGUI.alignment = TextAlignmentOptions.Center;

            EditorUtility.SetDirty(textMeshProUGUI);
        }
    }
}