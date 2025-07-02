using CustomUtils.Editor.Extensions;
using CustomUtils.Runtime.UI.HorizontalVerticalLayout;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor
{
    /// <inheritdoc />
    /// <summary>
    /// Custom Editor for the HorizontalOrVerticalLayoutGroupEditor Component.
    /// Extend this class to write a custom editor for a component derived from HorizontalOrVerticalLayoutGroupEditor.
    /// </summary>
    [CustomEditor(typeof(HorizontalVerticalLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class HorizontalVerticalLayoutGroupEditor : UnityEditor.Editor
    {
        private SerializedProperty _padding;
        private SerializedProperty _spacing;
        private SerializedProperty _childAlignment;
        private SerializedProperty _childControlWidth;
        private SerializedProperty _childControlHeight;
        private SerializedProperty _childScaleWidth;
        private SerializedProperty _childScaleHeight;
        private SerializedProperty _childForceExpandWidth;
        private SerializedProperty _childForceExpandHeight;
        private SerializedProperty _reverseArrangement;

        protected virtual void OnEnable()
        {
            _padding = serializedObject.FindProperty("m_Padding");
            _spacing = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.Spacing));
            _childAlignment = serializedObject.FindProperty("m_ChildAlignment");
            _childControlWidth = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildControlWidth));
            _childControlHeight = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildControlHeight));
            _childScaleWidth = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildScaleWidth));
            _childScaleHeight = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildScaleHeight));
            _childForceExpandWidth = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildForceExpandWidth));
            _childForceExpandHeight = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ChildForceExpandHeight));
            _reverseArrangement = serializedObject.FindField(nameof(HorizontalVerticalLayoutGroup.ReverseArrangement));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_padding, true);
            EditorGUILayout.PropertyField(_spacing, true);
            EditorGUILayout.PropertyField(_childAlignment, true);
            EditorGUILayout.PropertyField(_reverseArrangement, true);

            Rect rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Control Child Size"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _childControlWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, _childControlHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Use Child Scale"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _childScaleWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, _childScaleHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Child Force Expand"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, _childForceExpandWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, _childForceExpandHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggle = property.boolValue;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.ToggleLeft(position, label, toggle);
            EditorGUI.indentLevel = oldIndent;
            if (EditorGUI.EndChangeCheck())
                property.boolValue = property.hasMultipleDifferentValues || !property.boolValue;

            EditorGUI.EndProperty();
        }
    }
}