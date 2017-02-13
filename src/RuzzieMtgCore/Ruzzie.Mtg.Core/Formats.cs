using System;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper for the <see cref="Format"/> enum.
    /// </summary>
    public static class Formats
    {
        /// <summary>
        /// Determines whether the specified contains <b>any</b> of the formatToCheck flags.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatToCheck">The format to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified format to check contains format; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("This is replaced by the ContainsAnyFormat function.")]
        public static bool ContainsFormat(this Format format, Format formatToCheck)
        {
            return ContainsAnyFormat(format, formatToCheck);
        }

        /// <summary>
        /// Determines whether the specified contains <b>any</b> of the formatToCheck flags.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatToCheck">The format to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified format to check contains format; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAnyFormat(this Format format, Format formatToCheck)
        {
            if ((format & formatToCheck) != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a format contains <b>all</b> of the formatToCheckFlags.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatToCheck">The format to check.</param>
        /// <returns>
        ///   <c>true</c> if [contains format exact] [the specified format to check]; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAllFormat(this Format format, Format formatToCheck)
        {
            return (format & formatToCheck) == formatToCheck;            
        }
    }
}