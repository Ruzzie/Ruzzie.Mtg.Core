namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Defines the result of a lookup for object of type {T}  by name;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INameLookupResult<out T>
    {
        /// <summary>
        /// When found the object, null or default otherwise.
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

        /// <summary>
        /// Gets or sets the match probability. This is a range between 0 and 1.
        /// </summary>
        /// <value>
        /// The match probability.
        /// </value>
        double MatchProbability { get; set; }
    }
}