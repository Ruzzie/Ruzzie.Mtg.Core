using System;
using System.Collections.Concurrent;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper methods for the <see cref="BasicType"/> enum.
    /// </summary>
    public static class BasicTypes
    {
        private static readonly ConcurrentDictionary<string,BasicType> EnumNameCache = new ConcurrentDictionary<string, BasicType>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Parses a string with al the types and returns a <see cref="BasicType"/> enum for all matching basic types.
        /// </summary>
        /// <param name="typesString">The types string delimeted by space dash space.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Value cannot be null or whitespace.</exception>
        public static BasicType From(string typesString)
        {
            if (string.IsNullOrWhiteSpace(typesString))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(typesString));
            }

            return EnumNameCache.GetOrAdd(typesString, FromUncached);
        }

        private static BasicType FromUncached(string typesString)
        {
            string[] types = typesString.Split(new[] {" — "}, StringSplitOptions.RemoveEmptyEntries);

            return From(types);
        }

        /// <summary>
        /// Parses a string  arraywith al the types and returns a <see cref="BasicType"/> enum for all matching basic types.
        /// </summary>
        /// <param name="types">All types as a string array with not delimited by space dash space.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static BasicType From(string[] types)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            BasicType currentBasicTypes = BasicType.None;

            for (int i = 0; i < types.Length; i++)
            {
                string currentTypeString = types[i];
                currentBasicTypes |= EnumNameCache.GetOrAdd(currentTypeString,FromSingleTypeStringLine);
            }

            return currentBasicTypes;
        }

        private static BasicType FromSingleTypeStringLine(string currentTypeString)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(currentTypeString.Trim(), "Basic Land") || StringComparer.OrdinalIgnoreCase.Equals(currentTypeString.Trim(), "Basic Snow Land"))
            {
                return BasicType.BasicLand;               
            }

            var typeWords = currentTypeString.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            BasicType currentBasicType = BasicType.None;
            for (int j = 0; j < typeWords.Length; j++)
            {
                BasicType enumResult;
                if (Enum.TryParse(typeWords[j], true, out enumResult))
                {
                    currentBasicType |= enumResult;
                }
            }
            return currentBasicType;
        }

        /// <summary>
        /// Determines whether [contains basic type] [the specified basic type to check].
        /// </summary>
        /// <param name="basicType">Type of the basic.</param>
        /// <param name="basicTypeToCheck">The basic type to check.</param>
        /// <returns>
        ///   <c>true</c> if [contains basic type] [the specified basic type to check]; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsBasicType(this BasicType basicType, BasicType basicTypeToCheck)
        {
            if ((basicType & basicTypeToCheck) != 0)
            {
                return true;
            }

            return false;
        }
    }
}