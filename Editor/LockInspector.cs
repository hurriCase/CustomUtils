using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    /// <summary>
    /// Toggles the Inspector lock state and the Constrain Proportions lock state.
    /// </summary>
    internal static class LockInspector
    {
        private static readonly MethodInfo _flipLocked;
        private static readonly PropertyInfo _constrainProportions;
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        static LockInspector()
        {
#if UNITY_2023_2_OR_NEWER
            var editorLockTrackerType =
                typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.EditorGUIUtility+EditorLockTracker");
            _flipLocked = editorLockTrackerType.GetMethod("FlipLocked", BindingFlags);
#endif
            _constrainProportions = typeof(Transform)
                .GetProperty("constrainProportionsScale", BindingFlags);
        }

        [MenuItem("Edit/Toggle Inspector Lock %l")]
        public static void Lock()
        {
#if UNITY_2023_2_OR_NEWER

            var inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            foreach (var lockTracker in Resources.FindObjectsOfTypeAll(inspectorWindowType)
                         .Select(inspectorWindow => inspectorWindowType
                             .GetField("m_LockTracker", BindingFlags)
                             ?.GetValue(inspectorWindow)))
            {
                _flipLocked?.Invoke(lockTracker, new object[] { });
            }
#else
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
#endif
            foreach (var activeEditor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (activeEditor.target is not Transform target)
                    continue;

                var currentValue = (bool)_constrainProportions.GetValue(target, null);
                _constrainProportions.SetValue(target, !currentValue, null);
            }

            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        [MenuItem("Edit/Toggle Inspector Lock %l", true)]
        public static bool Valid() => ActiveEditorTracker.sharedTracker.activeEditors.Length != 0;
    }
}