using System.Linq;
using System.Reflection;
using CustomUtils.Runtime.AnyType;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using ValueType = CustomUtils.Runtime.AnyType.ValueType;

namespace CustomUtils.Editor.MethodSerialization
{
    [CustomPropertyDrawer(typeof(SerializedCallback<>), true)]
    public class SerializedCallbackDrawerUI : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            var targetProp = property.FindPropertyRelative("_targetObject");
            ObjectField targetField = new("Target")
            {
                objectType = typeof(Object),
                bindingPath = targetProp.propertyPath
            };
            root.Add(targetField);

            var methodProp = property.FindPropertyRelative("_methodName");
            Button methodField = new()
            {
                text = string.IsNullOrEmpty(methodProp.stringValue) ? "Select Method" : methodProp.stringValue
            };
            root.Add(methodField);

            methodField.clicked += () =>
                ShowMethodDropdown(targetProp.objectReferenceValue, methodProp, property, methodField, root);

            var parametersProp = property.FindPropertyRelative("_parameters");
            var parametersContainer = new VisualElement();
            root.Add(parametersContainer);

            UpdateParameters(parametersProp, parametersContainer);

            property.serializedObject.ApplyModifiedProperties();

            return root;
        }

        private void ShowMethodDropdown(Object target, SerializedProperty methodProp, SerializedProperty property,
            Button methodButton, VisualElement root)
        {
            if (target is null)
                return;

            GenericMenu menu = new();
            var targetType = target.GetType();

            var callbackType = fieldInfo.FieldType;
            var genericType = callbackType.GetGenericArguments()[0];
            if (callbackType.IsGenericType is false)
                return;

            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.ReturnType == genericType)
                .ToArray();

            foreach (var method in methods)
            {
                menu.AddItem(
                    new GUIContent(method.Name),
                    false,
                    () =>
                    {
                        methodProp.stringValue = method.Name;
                        methodButton.text = method.Name;

                        var parametersProp = property.FindPropertyRelative("_parameters");
                        var parameters = method.GetParameters();
                        parametersProp.arraySize = parameters.Length;

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            var paramProp = parametersProp.GetArrayElementAtIndex(i);
                            var typeProp = paramProp.FindPropertyRelative("Type");
                            typeProp.enumValueIndex = (int)AnyValue.ValueTypeOf(parameters[i].ParameterType);
                        }

                        property.serializedObject.ApplyModifiedProperties();

                        var parametersContainer = root.Children().Last();
                        parametersContainer.Clear();
                        UpdateParameters(parametersProp, parametersContainer);
                    }
                );

                if (methods.Any() is false)
                    menu.AddDisabledItem(new GUIContent("No methods found"));

                menu.ShowAsContext();
            }
        }

        private void UpdateParameters(SerializedProperty parametersProp, VisualElement container)
        {
            if (parametersProp.isArray is false) return;

            for (var i = 0; i < parametersProp.arraySize; i++)
            {
                var parameter = parametersProp.GetArrayElementAtIndex(i);
                var typeProp = parameter.FindPropertyRelative("Type");

                var paramType = (ValueType)typeProp.enumValueIndex;
                VisualElement field;

                switch (paramType)
                {
                    case ValueType.Int:
                        var intProp = parameter.FindPropertyRelative("IntValue");
                        IntegerField intField = new($"Parameter {i + 1} (Int)");
                        intField.value = intProp.intValue;
                        intField.RegisterValueChangedCallback(
                            evt =>
                            {
                                intProp.intValue = evt.newValue;
                                parametersProp.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        field = intField;
                        break;

                    case ValueType.Float:
                        var floatProp = parameter.FindPropertyRelative("FloatValue");
                        FloatField floatField = new($"Parameter {i + 1} (Float)");
                        floatField.value = floatProp.floatValue;
                        floatField.RegisterValueChangedCallback(
                            evt =>
                            {
                                floatProp.floatValue = evt.newValue;
                                parametersProp.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        field = floatField;
                        break;

                    case ValueType.String:
                        var stringProp = parameter.FindPropertyRelative("StringValue");
                        TextField stringField = new($"Parameter {i + 1} (String)");
                        stringField.value = stringProp.stringValue;
                        stringField.RegisterValueChangedCallback(
                            evt =>
                            {
                                stringProp.stringValue = evt.newValue;
                                parametersProp.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        field = stringField;
                        break;

                    case ValueType.Bool:
                        var boolProp = parameter.FindPropertyRelative("BoolValue");
                        Toggle boolField = new($"Parameter {i + 1} (Bool)");
                        boolField.value = boolProp.boolValue;
                        boolField.RegisterValueChangedCallback(
                            evt =>
                            {
                                boolProp.boolValue = evt.newValue;
                                parametersProp.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        field = boolField;
                        break;

                    case ValueType.Vector3:
                        var vector3Prop = parameter.FindPropertyRelative("Vector3Value");
                        Vector3Field vector3Field = new($"Parameter {i + 1} (Vector3)");
                        vector3Field.value = vector3Prop.vector3Value;
                        vector3Field.RegisterValueChangedCallback(
                            evt =>
                            {
                                vector3Prop.vector3Value = evt.newValue;
                                parametersProp.serializedObject.ApplyModifiedProperties();
                            }
                        );
                        field = vector3Field;
                        break;

                    default:
                        field = new Label($"Parameter {i + 1}: Unsupported Type");
                        break;
                }

                container.Add(field);
            }
        }
    }
}