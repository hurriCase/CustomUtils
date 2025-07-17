using System;
using UnityEditor;

namespace CustomUtils.Editor.EmumArrayPropertyDrawer
{
    internal struct EnumArrayInfo
    {
        public SerializedProperty ValuesProperty { get; set; }
        public Type EnumType { get; set; }
        public string[] EnumNames { get; set; }
        public bool SkipFirst { get; set; }
        public int StartIndex { get; set; }

        public readonly bool IsValid => ValuesProperty != null && EnumType != null;
    }
}