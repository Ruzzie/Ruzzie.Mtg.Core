using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary>Defines a value constraint on a value type.</summary>
    /// <typeparam name="T">The value type to which the value constraint will apply.</typeparam>
    public interface IValueConstraint<in T> where T: struct, IEquatable<T>, IComparable<T>
    {
        /// <summary>Determines whether [is within constraint] [the specified value].</summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is within constraint] [the specified value]; otherwise, <c>false</c>.</returns>
        bool IsWithinConstraint(T value);
    }
}