namespace Ruzzie.Mtg.Core.Data
{
    /// <summary>
    /// String helper methods for the <see cref="CardNameLookup{TCard}"/>.
    /// </summary>
    public static class CardNameLookUpStringHelper
    {
        private static readonly bool[] AllowedCharactersWhenStrippingLookup;

        static CardNameLookUpStringHelper()
        {
            AllowedCharactersWhenStrippingLookup = new bool[65536];
            for (char c = '0'; c <= '9'; c++) AllowedCharactersWhenStrippingLookup[c] = true;
            for (char c = 'A'; c <= 'Z'; c++) AllowedCharactersWhenStrippingLookup[c] = true;
            for (char c = 'a'; c <= 'z'; c++) AllowedCharactersWhenStrippingLookup[c] = true;
            AllowedCharactersWhenStrippingLookup[' '] = true;//Allow spaces

        }

        /// <summary>
        /// Removes all special characters from the input. Allowed are A-Z, a-z, 0-9, space replaces dash with space.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>the stripped string</returns>
        public static string RemoveSpecialCharacters(this string input)
        {
            if (input == null)
            {
                return null;
            }

            int inputLength = input.Length;

            if (inputLength == 0)
            {
                return input;
            }

            char[] buffer = new char[inputLength];
            int index = 0;

            for (var i = 0; i < inputLength; ++i)
            {
                char c = input[i];

                //replace dash with space
                if (c == '-')
                {
                    c = ' ';
                }

                if (AllowedCharactersWhenStrippingLookup[c])
                {
                    buffer[index] = c;
                    index++;
                }
            }
            return new string(buffer, 0, index);
        }
    }
}