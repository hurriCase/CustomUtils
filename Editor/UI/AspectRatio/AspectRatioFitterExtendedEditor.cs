using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Editor.UI.AspectRatio
{
    [CustomEditor(typeof(AspectRatioFitter), true)]
    [CanEditMultipleObjects]
    internal sealed class AspectRatioFitterExtendedEditor : AspectRatioEditorBase<AspectRatioFitter>
    {
        private UnityEditor.Editor _defaultEditor;

        protected override void InitializeEditor()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var aspectRatioFitterEditorType = assembly.GetType("UnityEditor.UI.AspectRatioFitterEditor");

            if (aspectRatioFitterEditorType != null)
                _defaultEditor = CreateEditor(targets, aspectRatioFitterEditorType);
        }

        protected override void CleanupEditor()
        {
            if (!_defaultEditor)
                return;

            DestroyImmediate(_defaultEditor);
            _defaultEditor = null;
        }

        protected override void DrawMainInspector()
        {
            if (_defaultEditor)
                _defaultEditor.OnInspectorGUI();
            else
                DrawDefaultInspector();
        }

        protected override RectTransform GetRectTransform(AspectRatioFitter component)
            => component.GetComponent<RectTransform>();

        protected override void SetAspectRatio(AspectRatioFitter component, float aspectRatio)
            => component.aspectRatio = aspectRatio;
    }
}