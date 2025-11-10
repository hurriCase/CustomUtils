using System.Reflection;
using UnityEditor;

namespace CustomUtils.Editor.Scripts
{
    internal static class SerializedPropertyClipboard
    {
        private static readonly MethodInfo _setSerializedPropertyMethod;
        private static readonly MethodInfo _getSerializedPropertyMethod;
        private static readonly MethodInfo _hasSerializedPropertyMethod;

        static SerializedPropertyClipboard()
        {
            var clipboardType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Clipboard");

            if (clipboardType == null)
                return;

            _setSerializedPropertyMethod = clipboardType.GetMethod(
                "SetSerializedProperty",
                BindingFlags.Public | BindingFlags.Static
            );

            _getSerializedPropertyMethod = clipboardType.GetMethod(
                "GetSerializedProperty",
                BindingFlags.Public | BindingFlags.Static
            );

            _hasSerializedPropertyMethod = clipboardType.GetMethod(
                "HasSerializedProperty",
                BindingFlags.Public | BindingFlags.Static
            );
        }

        internal static void CopyTo(this SerializedProperty property)
        {
            _setSerializedPropertyMethod?.Invoke(null, new object[] { property });
        }

        internal static void Paste(this SerializedProperty property)
        {
            _getSerializedPropertyMethod?.Invoke(null, new object[] { property });
        }

        internal static bool CanPaste()
        {
            var result = _hasSerializedPropertyMethod?.Invoke(null, null);
            return result is true;
        }
    }
}