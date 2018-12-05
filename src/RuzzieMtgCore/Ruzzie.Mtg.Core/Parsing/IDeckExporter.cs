using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public interface IDeckExporter<out TOutput, TCard> where TCard : IBasicCardProperties
    {
        TOutput Export(in DeckCards<TCard> cards);
    }
}