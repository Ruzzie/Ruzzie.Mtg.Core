using System;
using System.Runtime.CompilerServices;
using Ruzzie.Common.Hashing;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper methods for key generation.
    /// </summary>
    public static class KeyHelpers
    {
        private static readonly FNV1AHashAlgorithm64 HashAlgo64 = new FNV1AHashAlgorithm64();

        /// <summary>
        /// Creates a valid rowkey. The input value will be uppercased and // will be replaced by underscores
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="forwardSlashesReplaceValue"></param>
        /// <returns></returns>
        public static string CreateValidUpperCaseKeyForString(this string value, string forwardSlashesReplaceValue = "_")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            }
            if (string.IsNullOrWhiteSpace(forwardSlashesReplaceValue))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(forwardSlashesReplaceValue));
            }

            return value.Replace("/", "_").ToUpperInvariant();
        }

        /// <summary>
        /// Generates the bidirectonal key for two card name comparisons. the strings are first put through the <see cref="CreateValidUpperCaseKeyForString"/> function.
        /// </summary>
        /// <param name="firstString">The first string.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns></returns>
        public static ulong GenerateBidirectonalKeyForTwoCardNameComparisons(this string firstString, string secondString)
        {
            return GenerateBidirectonalKeyForTwoStringComparisons(firstString.CreateValidUpperCaseKeyForString(),
                secondString.CreateValidUpperCaseKeyForString());
        }

        /// <summary>
        /// Generates the bidirectonal key for two string comparisons.
        /// </summary>
        /// <param name="firstString">The first string.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns></returns>
        public static ulong GenerateBidirectonalKeyForTwoStringComparisons(this string firstString, string secondString)
        {
            ulong firstStringHash = firstString.GenerateHash64ForStringCaseInsensitive();
            ulong secondStringHash = secondString.GenerateHash64ForStringCaseInsensitive();
            return firstStringHash.GenerateBidirectonalKeyForTwoULongs(secondStringHash);
        }

        /// <summary>
        /// Generates the bidirectonal key for two u longs.
        /// </summary>
        /// <param name="firstUlong">The first ulong.</param>
        /// <param name="secondUlong">The second ulong.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GenerateBidirectonalKeyForTwoULongs(this ulong firstUlong, ulong secondUlong)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GenerateHash64ForStringCaseInsensitive(this string value )
        {
            return (ulong) HashAlgo64.HashStringCaseInsensitive(value);
        }
    }
}
