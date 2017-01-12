namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// Name lookup result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Data.INameLookupResult{T}" />
    public class NameLookupResult<T> : INameLookupResult<T> where T : IHasName
    {
        /// <summary>
        /// When found the object, null or default otherwise.
        /// </summary>
        /// <value>
        /// The result object.
        /// </value>
        public T ResultObject { get; set; }
        /// <summary>
        /// Gets or sets the match result.
        /// </summary>
        /// <value>
        /// The match result.
        /// </value>
        public LookupMatchResult MatchResult { get; set; }
    }
}