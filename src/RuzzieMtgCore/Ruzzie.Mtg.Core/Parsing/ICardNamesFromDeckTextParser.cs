namespace Ruzzie.Mtg.Core.Parsing
{
    public interface ICardNamesFromDeckTextParser
    {
        CardParseResult Parse(string cardsText, DeckTextParseOptions deckTextParseOptions = DeckTextParseOptions.SkipSideboard);
    }
}