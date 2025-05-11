using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CustomUtils.Runtime.AnyType;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Editor.MethodSerialization
{
    [Serializable]
    public class SerializedCallback<TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField] private Object _targetObject;
        [SerializeField] private string _methodName;
        [SerializeField] private AnyValue[] _parameters;

        [NonSerialized] private Delegate _cachedDelegate;
        [NonSerialized] private bool _isDelegateRebuilt;

        public TReturn Invoke() => Invoke(_parameters);

        public TReturn Invoke(params AnyValue[] args)
        {
            if (_isDelegateRebuilt is false)
                BuildDelegate();

            if (_cachedDelegate != null)
            {
                var result = _cachedDelegate.DynamicInvoke(ConvertParameters(args));
                return (TReturn)Convert.ChangeType(result, typeof(TReturn));
            }

            Debug.LogWarning($"Unable to invoke method {_methodName} on {_targetObject}");
            return default;
        }

        private object[] ConvertParameters(AnyValue[] args)
        {
            if (args == null || args.Length == 0) return Array.Empty<object>();

            var convertedParams = new object[args.Length];
            for (var i = 0; i < args.Length; i++)
                convertedParams[i] = args[i].ConvertValue<object>();

            return convertedParams;
        }

        private void BuildDelegate()
        {
            _cachedDelegate = null;

            if (_targetObject is null || string.IsNullOrEmpty(_methodName))
            {
                Debug.LogWarning("Target object or method name is null, cannot rebuild delegate.");
                return;
            }

            var targetType = _targetObject.GetType();
            var methodInfo = targetType.GetMethod(_methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                Debug.LogWarning($"Method {_methodName} not found on {_targetObject}");
                return;
            }

            var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            if (_parameters.Length != parameterTypes.Length)
            {
                Debug.LogWarning($"Parameter mismatch for method {_methodName}");
                return;
            }

            var delegateType = Expression.GetDelegateType(parameterTypes.Append(methodInfo.ReturnType).ToArray());
            _cachedDelegate = methodInfo.CreateDelegate(delegateType, _targetObject);
            _isDelegateRebuilt = true;
        }

        public void OnBeforeSerialize()
        {
            // noop
        }

        public void OnAfterDeserialize()
        {
            _isDelegateRebuilt = false;
        }
    }
}