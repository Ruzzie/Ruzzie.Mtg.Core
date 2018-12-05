using System;
using System.Collections.Generic;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public interface IDeckCardNameCleaner<TCard> where TCard : IBasicCardProperties
    {
        List<CardNameAndCount> CleanUpCards(in IReadOnlyList<CardNameAndCount> cardList);
        DeckCards<TCard> CleanUpCards(in IReadOnlyList<CardNameAndCount> mainboard, in IReadOnlyList<CardNameAndCount> sideboard);
    }

    public class DeckCards<TCard> where TCard : IBasicCardProperties
    {
        public DeckCards()
        {

        }

        public DeckCards(in List<DeckCard<TCard>> mainboard, in List<DeckCard<TCard>> sideboard)
        {
            Mainboard = mainboard ?? throw new ArgumentNullException(nameof(mainboard));
            Sideboard = sideboard ?? throw new ArgumentNullException(nameof(sideboard));
        }

        public List<DeckCard<TCard>> Mainboard { get; } = new List<DeckCard<TCard>>();
        public List<DeckCard<TCard>> Sideboard { get; } = new List<DeckCard<TCard>>();
    }
}
