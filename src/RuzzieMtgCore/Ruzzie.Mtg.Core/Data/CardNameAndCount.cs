using System;

namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Structure for capturing card name and the count of the card.
    /// </summary>
    public struct CardNameAndCount : IEquatable<CardNameAndCount>
    {
        /// <summary>Initializes a new instance of the <see cref="CardNameAndCount"/> struct.</summary>
        /// <param name="name">The name.</param>
        /// <param name="count">The count.</param>
        public CardNameAndCount(string name, int count)
        {
            Name = name;
            Count = count;
        }
        public string Name { get; }
        public int Count { get; set; }

        /// <inheritdoc />
        public bool Equals(CardNameAndCount other)
        {
#if HAVE_STRINGCOMPARISONINVARIANTCULTURE
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
#else
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
#endif
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CardNameAndCount && Equals((CardNameAndCount) obj);
        }

        /// <summary>Returns the hash code for this instance. This is soley based on the card name</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
#if HAVE_STRINGCOMPARISONINVARIANTCULTURE
            return (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
#else
            return (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
#endif
        }
        /// <inheritdoc />
        public static bool operator ==(CardNameAndCount left, CardNameAndCount right)
        {
            return left.Equals(right);
        }
        /// <inheritdoc />
        public static bool operator !=(CardNameAndCount left, CardNameAndCount right)
        {
            return !left.Equals(right);
        }
    }
}