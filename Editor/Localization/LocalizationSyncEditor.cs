using CustomUtils.Runtime.Localization;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.Localization
{
    /// <inheritdoc />
    /// <summary>
    ///     Adds "Sync" button to LocalizationSync script.
    /// </summary>
    [CustomEditor(typeof(LocalizationSync))]
    internal sealed class LocalizationSyncEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var component = (LocalizationSync)target;

            if (GUILayout.Button("Sync"))
                component.Sync();
        }
    }
}