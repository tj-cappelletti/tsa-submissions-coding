using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

        // Delegate to derived class for actual comparison logic
        return EqualsCore(x, y);
    }

    public bool Equals(IList<T>? x, IList<T>? y)
    {
        // Handle reference equality and null cases
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.Count != y.Count) return false;

        // Delegate to derived class for actual comparison logic
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

    protected abstract bool EqualsCore(IList<T> x, IList<T> y);

    /// <summary>
    ///     Returns a hash code for the specified object.
    /// </summary>
    /// <param name="obj">The object for which to get a hash code</param>
    /// <returns>A hash code for the specified object</returns>
    public abstract int GetHashCode(T? obj);

    public abstract int GetHashCode(IList<T>? obj);
}
