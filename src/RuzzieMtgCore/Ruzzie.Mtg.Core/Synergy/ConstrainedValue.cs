using System;

namespace Ruzzie.Mtg.Core.Synergy
{
    /// <summary>Represents a specific constrained value type</summary>
    /// <typeparam name="T">The type that is constraint</typeparam>
    /// <typeparam name="TConstraint">The constraint that should be applied to the type</typeparam>
    public struct ConstrainedValue<T,TConstraint> 
        : IEquatable<ConstrainedValue<T, TConstraint>>, IComparable<ConstrainedValue<T, TConstraint>>, IValueConstrainable<T>
        where T : struct, IEquatable<T>, IComparable<T>      
        where TConstraint : IValueConstraint<T>, new()
    {
        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public T Value { get; }

        /// <summary>Performs an implicit conversion from <see cref="ConstrainedValue{T, TConstraint}"/> to <see cref="T"/>.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T(ConstrainedValue<T, TConstraint>  value)
        {
            return value.Value;
        }

        /// <summary>Performs an implicit conversion from <see cref="T"/> to <see cref="ConstrainedValue{T, TConstraint}"/>.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ConstrainedValue<T, TConstraint> (T value)
        {
            return new ConstrainedValue<T, TConstraint>(value);
        }

        /// <summary>Initializes a new instance of the <see cref="ConstrainedValue{T, TConstraint}"/> struct.</summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException">value - Value is out of constrained range.</exception>
        public ConstrainedValue(T value)
        {
            if (!TypeConstraints<T, TConstraint>.Constraint.IsWithinConstraint(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value is out of constrained range.");
            }

            Value = value;
        }

        /// <summary>Indicates whether the current value is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current value is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ConstrainedValue<T, TConstraint> other)
        {
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(ConstrainedValue<T, TConstraint> other)
        {
            return Value.CompareTo(other.Value);
        }

        /// <summary>Gets the value constraint.</summary>
        /// <returns></returns>
        public IValueConstraint<T> GetValueConstraint()
        {
            return TypeConstraints<T, TConstraint>.Constraint;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ConstrainedValue<T, TConstraint> other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ConstrainedValue<T, TConstraint> left, ConstrainedValue<T, TConstraint> right)
        {
            return left.Equals(right);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ConstrainedValue<T, TConstraint> left, ConstrainedValue<T, TConstraint> right)
        {
            return !left.Equals(right);
        }
    }
}