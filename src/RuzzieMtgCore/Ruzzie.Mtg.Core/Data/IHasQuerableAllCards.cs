using System.Linq;

namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Interface to indicate the AllCards property, this is a required type for the <see cref="CardNameLookup{TCard}"/>
    /// </summary>
    /// <typeparam name="TCard">The type of the card.</typeparam>
    public interface IHasQuerableAllCards<out TCard> where TCard : IHasName
    {
        /// <summary>
        /// Gets all cards.
        /// </summary>
        /// <value>
        /// All cards.
        /// </value>
        IQueryable<TCard> AllCards { get; }
    }
}