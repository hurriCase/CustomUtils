using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Editor.UI.CustomComponents.ProceduralUIImage
{
    internal static class ProceduralImageEditorUtility
    {
        [MenuItem("CONTEXT/Image/Replace with Procedural Image")]
        internal static void ReplaceWithProceduralImage(MenuCommand command)
        {
            var image = (Image)command.context;
            var targetObject = image.gameObject;
            Object.DestroyImmediate(image);
            targetObject.AddComponent<ProceduralImage>();
        }
    }
}