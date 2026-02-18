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
    ///     Determines whether two lists are equal by comparing each element.
    /// </summary>
    /// <param name="x">The first list to compare</param>
    /// <param name="y">The second list to compare</param>
    /// <returns>True if the lists are equal; otherwise, false</returns>
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
    ///     When overridden in a derived class, performs the actual equality comparison for lists
    ///     after reference and null checks have been performed.
    /// </summary>
    /// <param name="x">The first list to compare (guaranteed to be non-null)</param>
    /// <param name="y">The second list to compare (guaranteed to be non-null)</param>
    /// <returns>True if the lists are equal; otherwise, false</returns>
    protected abstract bool EqualsCore(IList<T> x, IList<T> y);

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
    public int GetHashCode(IList<T>? obj)
    {
        if (obj is null) return 0;

        return GetOrderedHashCode(obj, GetOrderByKey);
    }

    /// <summary>
    ///     When overridden in a derived class, provides the key selector function for ordering items
    ///     when computing list hash codes.
    /// </summary>
    /// <param name="item">The item to extract the ordering key from</param>
    /// <returns>The key to use for ordering</returns>
    /// <remarks>
    ///     This ensures consistent hash codes for lists regardless of the order items are added.
    ///     The default implementation returns the item itself, which works for comparable types.
    ///     Override this to specify a custom ordering key.
    /// </remarks>
    protected virtual object GetOrderByKey(T item)
    {
        return item!;
    }

    /// <summary>
    ///     Computes a hash code for a list by ordering items and combining their individual hash codes.
    /// </summary>
    /// <param name="list">The list to compute a hash code for</param>
    /// <param name="orderByKeySelector">Function to extract the ordering key from each item</param>
    /// <returns>A hash code for the list</returns>
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
