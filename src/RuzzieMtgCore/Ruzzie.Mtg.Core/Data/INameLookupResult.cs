namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Defines the result of a lookup for object of type {T}  by name;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INameLookupResult<out T>
    {
        /// <summary>
        /// When found the object, null or default otherwist
        /// </summary>
        /// <value>
        /// The result object.
        /// </value>
        T ResultObject { get; }
        /// <summary>
        /// Gets or sets the match result.
        /// </summary>
        /// <value>
        /// The match result.
        /// </value>
        LookupMatchResult MatchResult { get; set; }
    }
}