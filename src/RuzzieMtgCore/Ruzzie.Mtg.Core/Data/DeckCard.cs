namespace Ruzzie.Mtg.Core.Data
{
    public interface IDeckCard<out TCard> where TCard : IHasName
    {
        TCard Card { get; }
        int Count { get;  }
    }

    public struct DeckCard<TCard> : IDeckCard<TCard> where TCard : IHasName
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