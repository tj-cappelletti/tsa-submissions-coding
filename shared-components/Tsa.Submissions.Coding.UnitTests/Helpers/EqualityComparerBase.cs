using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

/// <summary>
///     Provides a base class for equality comparers that handles common reference and null checks.
/// </summary>
/// <typeparam name="T">The type of objects to compare</typeparam>
[ExcludeFromCodeCoverage]
internal abstract class EqualityComparerBase<T> : IEqualityComparer<T?>, IEqualityComparer<IList<T>?>
{
    /// <summary>
    ///     Determines whether two objects of type <typeparamref name="T" /> are equal.
    /// </summary>
    /// <param name="x">The first object to compare</param>
    /// <param name="y">The second object to compare</param>
    /// <returns>True if the objects are equal; otherwise, false</returns>
    public bool Equals(T? x, T? y)
    {
        // Handle reference equality and null cases
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;

        // Delegate to derived class for actual comparison logic
        return EqualsCore(x, y);
    }

    /// <summary>
    ///     Determines whether two lists are equal by comparing each element using the predicate from
    ///     <see cref="GetItemPredicate" />.
    /// </summary>
    /// <param name="x">The first list to compare</param>
    /// <param name="y">The second list to compare</param>
    /// <returns>True if the lists are equal; otherwise, false</returns>
    /// <remarks>
    ///     This method performs order-independent comparison by using <see cref="GetItemPredicate" /> to match
    ///     corresponding items between the two lists based on their key properties.
    /// </remarks>
    public bool Equals(IList<T>? x, IList<T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        return EqualsCore(x, y);
    }

    /// <summary>
    ///     When overridden in a derived class, performs the actual equality comparison
    ///     after reference and null checks have been performed.
    /// </summary>
    /// <param name="x">The first object to compare (guaranteed to be non-null)</param>
    /// <param name="y">The second object to compare (guaranteed to be non-null)</param>
    /// <returns>True if the objects are equal; otherwise, false</returns>
    protected abstract bool EqualsCore(T x, T y);

    /// <summary>
    ///     Performs the actual equality comparison for lists after reference and null checks have been performed.
    ///     Compares lists in an order-independent manner by matching items using <see cref="GetItemPredicate" />.
    /// </summary>
    /// <param name="x">The first list to compare (guaranteed to be non-null and equal count to <paramref name="y" />)</param>
    /// <param name="y">The second list to compare (guaranteed to be non-null and equal count to <paramref name="x" />)</param>
    /// <returns>True if all items in <paramref name="x" /> have matching items in <paramref name="y" />; otherwise, false</returns>
    /// <remarks>
    ///     For each item in list <paramref name="x" />, this method uses <see cref="GetItemPredicate" /> to find
    ///     the corresponding item in list <paramref name="y" /> and compares them using <see cref="Equals(T, T)" />.
    /// </remarks>
    protected bool EqualsCore(IList<T> x, IList<T> y)
    {
        foreach (var leftItem in x)
        {
            var rightItem = y.SingleOrDefault(GetItemPredicate(leftItem));

            if (!Equals(leftItem, rightItem)) return false;
        }

        return true;
    }

    /// <summary>
    ///     Returns a hash code for the specified object.
    /// </summary>
    /// <param name="obj">The object for which to get a hash code</param>
    /// <returns>A hash code for the specified object</returns>
    public abstract int GetHashCode(T? obj);

    /// <summary>
    ///     Returns a hash code for a list by combining hash codes of its elements in a specified order.
    /// </summary>
    /// <param name="obj">The list for which to get a hash code</param>
    /// <returns>A hash code for the specified list</returns>
    /// <remarks>
    ///     Uses <see cref="GetOrderByKey" /> to order items before computing the hash code, ensuring
    ///     consistent hash codes regardless of the order items were added to the list.
    /// </remarks>
    public int GetHashCode(IList<T>? obj)
    {
        if (obj is null) return 0;

        return GetOrderedHashCode(obj, GetOrderByKey);
    }

    /// <summary>
    ///     When overridden in a derived class, provides a predicate function to find matching items in a list
    ///     based on key properties.
    /// </summary>
    /// <param name="item">The item to create a predicate for</param>
    /// <returns>A predicate function that matches items based on key properties</returns>
    /// <remarks>
    ///     This predicate is used by <see cref="EqualsCore(IList{T}, IList{T})" /> to perform order-independent
    ///     list comparisons. The predicate should match items based on their identifying properties (e.g., ID, composite
    ///     keys).
    ///     The same properties used here should typically be used in <see cref="GetOrderByKey" /> for consistency.
    /// </remarks>
    protected abstract Func<T, bool> GetItemPredicate(T item);

    /// <summary>
    ///     When overridden in a derived class, provides the key selector function for ordering items
    ///     when computing list hash codes.
    /// </summary>
    /// <param name="item">The item to extract the ordering key from</param>
    /// <returns>The key to use for ordering</returns>
    /// <remarks>
    ///     <para>
    ///         This ensures consistent hash codes for lists regardless of the order items are added.
    ///         Override this to specify the ordering key based on the item's identifying properties.
    ///     </para>
    ///     <para>
    ///         For simple sorting scenarios, return a single property such as <c>item.Id</c>.
    ///         For complex sorting scenarios involving multiple properties, return a tuple such as
    ///         <c>(item.Property1, item.Property2)</c> to sort by multiple keys in order.
    ///     </para>
    ///     <para>
    ///         The same properties used here should typically match those used in <see cref="GetItemPredicate" /> 
    ///         for consistency.
    ///     </para>
    /// </remarks>
    protected abstract object GetOrderByKey(T item);

    /// <summary>
    ///     Computes a hash code for a list by ordering items and combining their individual hash codes.
    /// </summary>
    /// <param name="list">The list to compute a hash code for</param>
    /// <param name="orderByKeySelector">Function to extract the ordering key from each item</param>
    /// <returns>A hash code for the list</returns>
    /// <remarks>
    ///     Items are ordered using the <paramref name="orderByKeySelector" /> before computing the hash code
    ///     to ensure consistent hash codes for lists containing the same items in different orders.
    /// </remarks>
    protected int GetOrderedHashCode(IList<T> list, Func<T, object> orderByKeySelector)
    {
        var hash = new HashCode();

        foreach (var item in list.OrderBy(orderByKeySelector))
        {
            hash.Add(GetHashCode(item));
        }

        return hash.ToHashCode();
    }
}
