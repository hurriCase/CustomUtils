using UnityEditor;

namespace CustomUtils.Editor.Scripts.EmumArrayPropertyDrawer
{
    internal readonly struct EnumArrayInfo
    {
        internal SerializedProperty EntriesProperty { get; }
        internal string[] EnumNames { get; }
        internal int StartIndex { get; }

        internal EnumArrayInfo(SerializedProperty entriesProperty, string[] enumNames, int startIndex)
        {
            EntriesProperty = entriesProperty;
            EnumNames = enumNames;
            StartIndex = startIndex;
        }
    }
}