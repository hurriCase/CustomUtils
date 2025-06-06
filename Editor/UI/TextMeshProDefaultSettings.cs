using TMPro;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI
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
            textMeshProUGUI.fontSizeMax = 300;
            textMeshProUGUI.alignment = TextAlignmentOptions.Center;

            EditorUtility.SetDirty(textMeshProUGUI);
        }
    }
}