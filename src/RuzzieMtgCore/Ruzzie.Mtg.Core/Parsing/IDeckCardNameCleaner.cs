using System.Collections.Generic;
using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public interface IDeckCardNameCleaner<TCard> where TCard : IBasicCardProperties
    {
        List<CardNameAndCount> CleanUpCards(List<CardNameAndCount> cardList);
        DeckCards<TCard> CleanUpCards(List<CardNameAndCount> mainboard, List<CardNameAndCount> sideboard);
    }

    public class DeckCards<TCard> where TCard:IBasicCardProperties
    {
        public List<DeckCard<TCard>> Mainboard { get; set; } = new List<DeckCard<TCard>>();
        public List<DeckCard<TCard>> Sideboard { get; set; } = new List<DeckCard<TCard>>();
    }
}
