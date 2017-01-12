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
        /// No format. (0_
        /// </summary>
        None = 0,
        //TODO: Add 1
        /// <summary>
        /// The standard format (2)
        /// </summary>
        Standard = 2,
        /// <summary>
        /// The modern format (4)
        /// </summary>
        Modern = 4,
        /// <summary>
        /// The legacy format (8)
        /// </summary>
        Legacy = 8,
        /// <summary>
        /// The vintage format (16)
        /// </summary>
        Vintage = 16,
        /// <summary>
        /// The pauper format (32)
        /// </summary>
        Pauper = 32,
        /// <summary>
        /// The penny format (64)
        /// </summary>
        Penny = 64,
        /// <summary>
        /// The duel format (128)
        /// </summary>
        Duel = 128,
        /// <summary>
        /// The commander format (256)
        /// </summary>
        Commander = 256,
        /// <summary>
        /// The frontier format (512)
        /// </summary>
        Frontier = 512
    }
}