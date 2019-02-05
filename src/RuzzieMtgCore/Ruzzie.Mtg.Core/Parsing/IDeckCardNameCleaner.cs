using System;
using System.Collections.Generic;
using System.Linq;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public interface IDeckCardNameCleaner<out TCard> where TCard : IHasName
    {
        List<CardNameAndCount> CleanUpCards(in IReadOnlyList<CardNameAndCount> cardList);
        IDeckCards<TCard> CleanUpCards(in IReadOnlyList<CardNameAndCount> mainboard, in IReadOnlyList<CardNameAndCount> sideboard);
    }

    public interface IDeckCards<out TCard> where TCard : IHasName
    {
        IEnumerable<IDeckCard<TCard>> Mainboard { get; }
        IEnumerable<IDeckCard<TCard>> Sideboard { get; }
    }

    public class DeckCards<TCard> : IDeckCards<TCard> where TCard : IHasName
    {
        public DeckCards()
        {

        }

        public DeckCards(in IEnumerable<IDeckCard<TCard>> mainboard, in IEnumerable<IDeckCard<TCard>> sideboard)
        {
            Mainboard = mainboard ?? throw new ArgumentNullException(nameof(mainboard));
            Sideboard = sideboard ?? throw new ArgumentNullException(nameof(sideboard));
        }

        public DeckCards(IEnumerable<DeckCard<TCard>> mainboard, IEnumerable<DeckCard<TCard>> sideboard)
        {
            Mainboard = mainboard.Cast<IDeckCard<TCard>>();
            Sideboard = sideboard.Cast<IDeckCard<TCard>>();
        }
      
        public IEnumerable<IDeckCard<TCard>> Mainboard { get; } = new List<IDeckCard<TCard>>();
        public IEnumerable<IDeckCard<TCard>> Sideboard { get; } = new List<IDeckCard<TCard>>();
    }
}
