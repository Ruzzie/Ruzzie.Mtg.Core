using System;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// An enum that represents playable formats for Mtg
    /// </summary>
    [Flags]
    public enum Format
    {
        /// <summary>
        /// No format.
        /// </summary>
        None = 0,
        /// <summary>
        /// The standard format
        /// </summary>
        Standard = 2,
        /// <summary>
        /// The modern format
        /// </summary>
        Modern = 4,
        /// <summary>
        /// The legacy format
        /// </summary>
        Legacy = 8,
        /// <summary>
        /// The vintage format
        /// </summary>
        Vintage = 16,
        /// <summary>
        /// The pauper format
        /// </summary>
        Pauper = 32,
        /// <summary>
        /// The penny format
        /// </summary>
        Penny = 64,
        /// <summary>
        /// The duel format
        /// </summary>
        Duel = 128,
        /// <summary>
        /// The commander format
        /// </summary>
        Commander = 256,
        /// <summary>
        /// The frontier format
        /// </summary>
        Frontier = 512
    }
}