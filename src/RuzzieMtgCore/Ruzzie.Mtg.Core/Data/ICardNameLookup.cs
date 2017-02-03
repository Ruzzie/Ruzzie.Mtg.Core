using System.Collections.Generic;

namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface for card lookups
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    public interface ICardNameLookup<TCard> where TCard: IHasName
    {
        /// <summary>
        /// Finds object matching the (casinsensitive) partialName. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The partialName of the object to find.</param>
        /// <param name="minProbability">THe minimum probability threshold for fuzzy matching. Should be between 0.75 and 0.99</param>
        /// <returns>The lookup result.</returns>
        INameLookupResult<TCard> FindCardByName(string name, double minProbability);

        /// <summary>
        /// Finds object matching the (casinsensitive) partialName. If an empty string is passed (also after a trim). The default(TCard) is returned.
        /// </summary>
        /// <param name="name">The partialName of the object to find.</param>
        /// <returns>The lookup result.</returns>
        INameLookupResult<TCard> FindCardByName(string name);

        /// <summary>
        /// Returns all probable matches for a partial name.
        /// </summary>
        /// <param name="partialName">The partialName.</param>
        /// <param name="minProbability"></param>
        /// <param name="maxresults">The maxresults.</param>
        /// <returns>the lookupresults.</returns>
        IEnumerable<INameLookupResult<TCard>> LookupCardName(string partialName, double minProbability, int maxresults = 10);
    }
}