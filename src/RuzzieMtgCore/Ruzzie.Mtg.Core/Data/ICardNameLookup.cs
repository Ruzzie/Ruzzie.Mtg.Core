namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface for card lookups
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    public interface ICardNameLookup<TCard> where TCard: IHasName
    {
        /// <summary>
        /// Finds object matching the (casinsensitive) name. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <param name="minProbability">THe minimum probability threshold for fuzzy matching. Should be between 0.75 and 0.99</param>
        /// <returns>The lookup result.</returns>
        INameLookupResult<TCard> FindCardByName(string name, double minProbability);

        /// <summary>
        /// Finds object matching the (casinsensitive) name. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The lookup result.</returns>
        INameLookupResult<TCard> FindCardByName(string name);
    }
}