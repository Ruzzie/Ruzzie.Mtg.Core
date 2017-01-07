namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper for the <see cref="Format"/> enum.
    /// </summary>
    public static class Formats
    {
        /// <summary>
        /// Determines whether the specified format to check contains format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatToCheck">The format to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified format to check contains format; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsFormat(this Format format, Format formatToCheck)
        {
            if ((format & formatToCheck) != 0)
            {
                return true;
            }

            return false;
        }
    }
}