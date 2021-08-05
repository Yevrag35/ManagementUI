using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ManagementUI.Functionality.Models.Extensions
{
    public static class IndexHelper
    {
        public static int GetPositiveIndex(int index, int totalCount)
        {
            return index < 0
                ? totalCount + index
                : index;
        }
    }
    //using ManagementUI.Functionality.Models.Extensions.Internal;
    

    public static class ListIndexExtensions
    {
        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the element of type <typeparamref name="T"/> at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="IList{T}"/> and count backwards.
        /// </remarks>
        /// <typeparam name="TList">
        ///     The type of <see cref="IList{T}"/> where <paramref name="index"/> 
        ///     will be used on.
        /// </typeparam>
        /// <typeparam name="TItem">The element type of <paramref name="list"/>.</typeparam>
        /// <param name="list">The list which the index will be verified against.</param>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The element of type <typeparamref name="TItem"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then the default value of <typeparamref name="TItem"/>.
        /// </returns>
        public static TItem GetByIndex<TItem, TList>(this TList list, int index)
            where TList : IList<TItem>
        {
            index = IndexHelper.GetPositiveIndex(index, list.Count);

            return index >= 0 && index < list.Count
                ? list[index]
                : default;
        }

        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the element of type <typeparamref name="T"/> at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="Collection{T}"/> and count backwards.
        /// </remarks>
        /// <typeparam name="TCollection">
        ///     The type of <see cref="Collection{T}"/> where <paramref name="index"/> 
        ///     will be used on.
        /// </typeparam>
        /// <typeparam name="TItem">The element type of <paramref name="collection"/>.</typeparam>
        /// <param name="collection">The collection which the index will be verified against.</param>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The element of type <typeparamref name="TItem"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then the default value of <typeparamref name="TItem"/>.
        /// </returns>
        public static TItem GetFromProperIndex<TCollection, TItem>(this TCollection collection, int index)
            where TCollection : Collection<TItem>
        {
            index = IndexHelper.GetPositiveIndex(index, collection.Count);

            return index >= 0 && index < collection.Count
                ? collection[index]
                : default;
        }

        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the <see cref="object"/> at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="IList"/> and count backwards.
        /// </remarks>
        /// <param name="list">The list which the index will be verified against.</param>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The <see cref="object"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then <see langword="null"/>.
        /// </returns>
        public static object GetFromProperIndex(IList list, int index)
        {
            index = IndexHelper.GetPositiveIndex(index, list.Count);

            return index >= 0 && index < list.Count
                ? list[index]
                : null;
        }
    }
}

namespace ManagementUI.Functionality.Models.Extensions.List
{
    //using ManagementUI.Functionality.Models.Extensions.Internal;

    public static class ListOnlyIndexExtensions
    {
        public static bool IsValidIndex<TItem>(this List<TItem> collection, int index)
        {
            return TryIsValidIndex(collection, index, out int throwAway);
        }

        public static bool TryIsValidIndex<TItem>(this List<TItem> collection, int index, out int positiveIndex)
        {
            positiveIndex = IndexHelper.GetPositiveIndex(index, collection.Count);
            return positiveIndex >= 0 && positiveIndex < collection.Count;
        }

        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the element of type <typeparamref name="T"/> at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="IList{T}"/> and count backwards.
        /// </remarks>
        /// <typeparam name="TList">
        ///     The type of <see cref="IList{T}"/> where <paramref name="index"/> 
        ///     will be used on.
        /// </typeparam>
        /// <typeparam name="TItem">The element type of <paramref name="list"/>.</typeparam>
        /// <param name="list">The list which the index will be verified against.</param>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The element of type <typeparamref name="TItem"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then the default value of <typeparamref name="TItem"/>.
        /// </returns>
        public static TItem GetByIndex<TItem>(this List<TItem> list, int index)
        {
            index = IndexHelper.GetPositiveIndex(index, list.Count);

            return index >= 0 && index < list.Count
                ? list[index]
                : default;
        }
    }
}

namespace ManagementUI.Functionality.Models.Extensions.ReadOnly
{
    //using ManagementUI.Functionality.Models.Extensions.Internal;
    public static class InterfaceReadOnlyListIndexExtensions
    {
        /// <summary>
        /// Transforms and verifies the specified negative or positive index into a proper <see cref="int"/> value
        /// returning the <typeparamref name="TItem"/> item at the proper index location.
        /// </summary>
        /// <remarks>
        ///     Used for transforming negative index <see cref="int"/> values into postive index positions.  When
        ///     negative indicies are specified, instead of starting the zero-based position, it will begin at the 
        ///     index of the last element of the <see cref="IReadOnlyList{T}"/> and count backwards.
        /// </remarks>
        /// <typeparam name="TReadOnlyList">
        ///     The type of <see cref="IReadOnlyList{T}"/> where <paramref name="index"/> 
        ///     will be used on.
        /// </typeparam>
        /// <typeparam name="TItem">The element type of <paramref name="list"/>.</typeparam>
        /// <param name="list">The list which the index will be verified against.</param>
        /// <param name="index">The negative or positive index value.</param>
        /// <returns>
        ///     The element of type <typeparamref name="TItem"/> at the specified proper index position; otherwise, 
        ///     if the index is determined to be out-of-range, then the default value of <typeparamref name="TItem"/>.
        /// </returns>
        public static TItem GetFromProperIndex<TReadOnlyList, TItem>(this TReadOnlyList list, int index)
            where TReadOnlyList : IReadOnlyList<TItem>
        {
            index = IndexHelper.GetPositiveIndex(index, list.Count);

            return index >= 0 && index < list.Count
                ? list[index]
                : default;
        }
    }
}
