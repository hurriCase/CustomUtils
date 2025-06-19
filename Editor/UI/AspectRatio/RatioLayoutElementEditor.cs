using CustomUtils.Runtime.UI.RatioLayout;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.UI.AspectRatio
{
    [CustomEditor(typeof(RatioLayoutElement), true)]
    [CanEditMultipleObjects]
    internal sealed class RatioLayoutElementEditor : AspectRatioEditorBase<RatioLayoutElement>
    {
        protected override void DrawMainInspector() => DrawDefaultInspector();

        protected override RectTransform GetRectTransform(RatioLayoutElement component)
            => component.RectTransform;

        protected override void SetAspectRatio(RatioLayoutElement component, float aspectRatio)
            => component.AspectRatio = aspectRatio;
    }
}