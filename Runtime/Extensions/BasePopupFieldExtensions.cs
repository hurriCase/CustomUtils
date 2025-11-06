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
        private const string UnityInspectorFieldUssClassName = "unity-inspector-field";

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
            field.pickingMode = PickingMode.Ignore;
            field.RegisterCallback<ClickEvent, (TSource source, Action<TSource> clickAction)>(
                static (_, tuple) => tuple.clickAction?.Invoke(tuple.source),
                (source, clickAction), TrickleDown.TrickleDown);

            if (field.TryQ<Label>(out var label))
                label.pickingMode = PickingMode.Ignore;

            var dropdownInput = field.Q(className: PopupField<string>.inputUssClassName);
            dropdownInput.pickingMode = PickingMode.Position;
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

        public static bool TryQ<TElement>(this VisualElement e,
            out TElement element,
            string name = null,
            string className = null)
            where TElement : VisualElement
        {
            element = e.Q<TElement>(name, className);
            return element != null;
        }
    }
}