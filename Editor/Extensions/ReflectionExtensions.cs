﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Editor.Extensions
{
    /// <summary>
    /// Provides extension methods for retrieving field names using reflection, primarily for editor tools.
    /// </summary>
    /// <remarks>
    /// These utilities help locate fields in classes based on their types, which is particularly useful
    /// for editor scripts that need to reference serialized properties by name without hardcoding strings.
    /// </remarks>
    public static class ReflectionExtensionsEditor
    {
        /// <summary>
        /// Gets the name of the first field of type <typeparamref name="TField"/> in the class <typeparamref name="TClass"/>.
        /// </summary>
        /// <typeparam name="TClass">The class type to search for fields.</typeparam>
        /// <typeparam name="TField">The field type to find.</typeparam>
        /// <param name="_">Placeholder parameter that enables extension method syntax. Not used.</param>
        /// <param name="bindingFlags">Binding flags that determine which fields to search. Defaults to non-public instance fields.</param>
        /// <returns>The name of the first field of type <typeparamref name="TField"/> found in the class.</returns>
        /// <remarks>
        /// This method is useful when you need to reference a field by name in serialized properties
        /// but want to avoid hardcoding strings. Throws an exception if no matching field is found.
        /// </remarks>
        [UsedImplicitly]
        public static string GetFieldName<TClass, TField>(this TClass _,
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
            where TClass : class =>
            typeof(TClass)
                .GetFields(bindingFlags)
                .AsValueEnumerable()
                .First(fieldInfo => fieldInfo.FieldType == typeof(TField))
                .Name;

        /// <summary>
        /// Gets the name of the first field of type List&lt;<typeparamref name="TElement"/>&gt; in the class <typeparamref name="TClass"/>.
        /// </summary>
        /// <typeparam name="TClass">The class type to search for fields.</typeparam>
        /// <typeparam name="TElement">The element type of the List field to find.</typeparam>
        /// <param name="_">Placeholder parameter that enables extension method syntax. Not used.</param>
        /// <param name="bindingFlags">Binding flags that determine which fields to search. Defaults to non-public instance fields.</param>
        /// <returns>The name of the first List&lt;<typeparamref name="TElement"/>&gt; field found in the class.</returns>
        /// <remarks>
        /// This method is useful when you need to reference a List field by name in serialized properties
        /// but want to avoid hardcoding strings. Throws an exception if no matching field is found.
        /// </remarks>
        [UsedImplicitly]
        public static string GetListFieldName<TClass, TElement>(this TClass _,
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
            where TClass : class =>
            typeof(TClass)
                .GetFields(bindingFlags)
                .First(fieldInfo => fieldInfo.FieldType == typeof(List<TElement>))
                .Name;
    }
}