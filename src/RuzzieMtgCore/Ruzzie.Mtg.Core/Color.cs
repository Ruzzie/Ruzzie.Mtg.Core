using System;

namespace Ruzzie.Mtg.Core
{
    [Flags]
    public enum Color
    {
        /// <summary>
        /// No color or identity is set, this means colorless.
        /// </summary>
        Colorless = 0,
        /// <summary>
        /// White
        /// </summary>
        W = 1,
        /// <summary>
        /// Blue
        /// </summary>
        U = 2,
        /// <summary>
        /// Black
        /// </summary>
        B = 4,
        /// <summary>
        /// Green
        /// </summary>
        G = 8,
        /// <summary>
        /// Red
        /// </summary>
        R = 16,          
    }
}