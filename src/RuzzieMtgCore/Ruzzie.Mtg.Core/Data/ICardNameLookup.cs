namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface for card lookups
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    public interface ICardNameLookup<TCard> where TCard: IHasName
    {
        /// <summary>
        /// Tries to find a matching TCard for the given input string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see cref="INameLookupResult{T}"/></returns>
        INameLookupResult<TCard> FindCardByName(string name);
    }
}