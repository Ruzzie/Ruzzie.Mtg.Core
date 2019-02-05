using Ruzzie.Mtg.Core.Data;

namespace Ruzzie.Mtg.Core.Parsing
{
    public interface IDeckExporter<out TOutput, TCard> where TCard : IHasName
    {
        TOutput Export(in IDeckCards<TCard> cards);
    }
}