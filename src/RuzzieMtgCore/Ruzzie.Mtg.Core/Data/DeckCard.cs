namespace Ruzzie.Mtg.Core.Data
{
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