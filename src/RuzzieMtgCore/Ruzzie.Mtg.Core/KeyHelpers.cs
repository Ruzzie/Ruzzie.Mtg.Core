using System.Runtime.CompilerServices;
using Ruzzie.Common.Hashing;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper methods for key generation.
    /// </summary>
    public static class KeyHelpers
    {
        public const string DefaultForwardSlashReplaceValue = "_";
        private static readonly FNV1AHashAlgorithm64 HashAlgo64 = new FNV1AHashAlgorithm64();

        /// <summary>
        /// Creates a valid string based row key. The input value will be upper cased and // will be replaced by underscores
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="forwardSlashesReplaceValue"></param>
        /// <returns></returns>
        public static string CreateValidUpperCaseKeyForString(this string value, string forwardSlashesReplaceValue = DefaultForwardSlashReplaceValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }          

            return value.Replace("/", forwardSlashesReplaceValue ?? DefaultForwardSlashReplaceValue).ToUpperInvariant();
        }

        /// <summary>
        /// Generates the bidirectional key for two card name comparisons. the strings are first put through the <see cref="CreateValidUpperCaseKeyForString"/> function.
        /// </summary>
        /// <param name="firstString">The first string.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns></returns>
        public static ulong GenerateBidirectionalKeyForTwoCardNameComparisons(this string firstString, string secondString)
        {
            return GenerateBidirectionalKeyForTwoStringComparisons(firstString.CreateValidUpperCaseKeyForString(),
                secondString.CreateValidUpperCaseKeyForString());
        }

        /// <summary>
        /// Generates the bidirectional key for two string comparisons.
        /// </summary>
        /// <param name="firstString">The first string.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns></returns>
        public static ulong GenerateBidirectionalKeyForTwoStringComparisons(this string firstString, string secondString)
        {
            ulong firstStringHash = firstString.GenerateHash64ForStringCaseInsensitive();
            ulong secondStringHash = secondString.GenerateHash64ForStringCaseInsensitive();
            return firstStringHash.GenerateBidirectionalKeyForTwoULongs(secondStringHash);
        }

        /// <summary>
        /// Generates the bidirectional key for two u longs.
        /// </summary>
        /// <param name="firstUlong">The first ulong.</param>
        /// <param name="secondUlong">The second ulong.</param>
        /// <returns></returns>
#if !PORTABLE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GenerateBidirectionalKeyForTwoULongs(this ulong firstUlong, ulong secondUlong)
        {
            unchecked
            {
                return ((firstUlong * secondUlong) ^ secondUlong) ^ firstUlong;
            }
        }

        /// <summary>
        /// Generates the hash64 for string case insensitive.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
#if !PORTABLE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GenerateHash64ForStringCaseInsensitive(this string value )
        {
            return (ulong) HashAlgo64.HashStringCaseInsensitive(value ?? string.Empty);
        }
    }
}
