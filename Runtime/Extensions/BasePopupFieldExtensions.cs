using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="VisualElement"/> and related types.
    /// </summary>
    [UsedImplicitly]
    public static class BasePopupFieldExtensions
    {
        private const string UnityInspectorFieldUssClassName = "inspector-field";

        /// <summary>
        /// Registers a click action on the popup field input element.
        /// </summary>
        /// <typeparam name="TValueType">The type of the value of the popup field.</typeparam>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <param name="field">The popup field to register the click action on.</param>
        /// <param name="source">The source object to pass to the click action.</param>
        /// <param name="clickAction">The action to invoke when the input is clicked.</param>
        [UsedImplicitly]
        public static void RegisterInputClick<TValueType, TSource>(
            this PopupField<TValueType> field,
            TSource source,
            Action<TSource> clickAction)
        {
            var inputFiend = field.Q(className: PopupField<string>.inputUssClassName);
            inputFiend.pickingMode = PickingMode.Position;
            inputFiend.RegisterCallback<ClickEvent, (TSource source, Action<TSource> clickAction)>(
                static (_, tuple) => tuple.clickAction?.Invoke(tuple.source),
                (source, clickAction), TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Adds Unity inspector field styling classes to the field.
        /// </summary>
        /// <typeparam name="TValueType">The type of the value of the field.</typeparam>
        /// <param name="field">The field to add styling classes to.</param>
        [UsedImplicitly]
        public static void AddUnityFileStyles<TValueType>(this BaseField<TValueType> field)
        {
            field.AddToClassList(BaseField<TValueType>.alignedFieldUssClassName);
            field.AddToClassList(UnityInspectorFieldUssClassName);
        }

        /// <summary>
        /// Attempts to query a child element of the specified type.
        /// </summary>
        /// <typeparam name="TElement">The type of the element to query for.</typeparam>
        /// <param name="element">The visual element to query from.</param>
        /// <param name="queriedElement">The queried element if found; otherwise, null.</param>
        /// <param name="name">The name of the element to query for.</param>
        /// <param name="className">The USS class name of the element to query for.</param>
        /// <returns>True if the element was found; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryQ<TElement>(
            this VisualElement element,
            out TElement queriedElement,
            string name = null,
            string className = null)
            where TElement : VisualElement
        {
            queriedElement = element.Q<TElement>(name, className);
            var wasFound = queriedElement != null;

#if UNITY_DEBUG
            if (wasFound is false)
                Debug.LogWarning($"[BasePopupFieldExtensions::TryQ] Element of {typeof(TElement)} type wasn't found");
#endif

            return wasFound;
        }
    }
}