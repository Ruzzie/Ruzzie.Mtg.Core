namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// The type of match for a lookup.
    /// </summary>
    public enum LookupMatchResult
    {
        /// <summary>
        /// No match was found.
        /// </summary>
        NoMatch,
        /// <summary>
        /// A match was found.
        /// </summary>
        Match,
        /// <summary>
        /// Indicates that a non exact match was found.
        /// </summary>
        FuzzyMatch
    }
}