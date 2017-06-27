using System;

namespace Ruzzie.Mtg.Core.Data
{
    //TODO: move to core library and implement caseinsensitive equality operators
    public struct CardNameAndCount : IEquatable<CardNameAndCount>
    {
        public CardNameAndCount(string name, int count)
        {
            Name = name;
            Count = count;
        }
        public string Name { get; }
        public int Count { get; set; }

        public bool Equals(CardNameAndCount other)
        {
#if HAVE_STRINGCOMPARISONINVARIANTCULTURE
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
#else
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
#endif
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CardNameAndCount && Equals((CardNameAndCount) obj);
        }

        public override int GetHashCode()
        {
#if HAVE_STRINGCOMPARISONINVARIANTCULTURE
            return (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
#else
            return (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
#endif
        }

        public static bool operator ==(CardNameAndCount left, CardNameAndCount right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CardNameAndCount left, CardNameAndCount right)
        {
            return !left.Equals(right);
        }
    }

    public struct DeckCard<TCard> where TCard : IHasName
    {
        public DeckCard(TCard card, int count)
        {
            Card = card;
            Count = count;
        }
        public TCard Card { get; }
        public int Count { get; set; }
    }
}