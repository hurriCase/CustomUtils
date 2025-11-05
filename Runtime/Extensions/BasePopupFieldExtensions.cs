using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="BasePopupField{TValueType, TValueChoice}"/>.
    /// </summary>
    [UsedImplicitly]
    public static class BasePopupFieldExtensions
    {
        private const string UnityInputFieldUSSClass = "unity-popup-field__input";

        /// <summary>
        /// Registers a click action on the popup field input element.
        /// </summary>
        /// <typeparam name="TValueType">The type of the value of the popup field.</typeparam>
        /// <typeparam name="TValueChoice">The type of the choices in the popup field.</typeparam>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <param name="field">The popup field to register the click action on.</param>
        /// <param name="source">The source object to pass to the click action.</param>
        /// <param name="clickAction">The action to invoke when the input is clicked.</param>
        [UsedImplicitly]
        public static void RegisterInputClick<TValueType, TValueChoice, TSource>(
            this BasePopupField<TValueType, TValueChoice> field,
            TSource source,
            Action<TSource> clickAction)
        {
            field.pickingMode = PickingMode.Ignore;
            field.RegisterCallback<ClickEvent, (TSource source, Action<TSource> clickAction)>(
                static (_, tuple) => tuple.clickAction?.Invoke(tuple.source),
                (source, clickAction), TrickleDown.TrickleDown);

            var label = field.Q<Label>();
            label.pickingMode = PickingMode.Ignore;

            var dropdownInput = field.Q(className: UnityInputFieldUSSClass);
            dropdownInput.pickingMode = PickingMode.Position;
        }
    }
}