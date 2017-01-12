using System;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Representation of one or more Colors for a Mtg Card.
    /// </summary>
    [Flags]
    public enum Color
    {
        /// <summary>
        /// No color or identity is set, this means colorless. (0)
        /// </summary>
        Colorless = 0,
        /// <summary>
        /// White (1)
        /// </summary>
        W = 1,
        /// <summary>
        /// Blue (2)
        /// </summary>
        U = 2,
        /// <summary>
        /// Black (4)
        /// </summary>
        B = 4,
        /// <summary>
        /// Green (8)
        /// </summary>
        G = 8,
        /// <summary>
        /// Red (16)
        /// </summary>
        R = 16,          
    }
}