using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ruzzie.Common.Collections;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper methods for the <see cref="BasicType"/> enum.
    /// </summary>
    public static class BasicTypes
    {
        private static readonly ConcurrentDictionary<string,BasicType> EnumNameCache = new ConcurrentDictionary<string, BasicType>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// All Basic land card names uppercased.
        /// A readonly <see cref="ReadOnlySet{String}"/> wrapped around a <see cref="HashSet{String}"/> for fast lookup.
        /// "ISLAND", "PLAINS", "SWAMP", "FOREST", "MOUNTAIN", "SNOW-COVERED ISLAND", "SNOW-COVERED PLAINS", "SNOW-COVERED SWAMP", "SNOW-COVERED FOREST", "SNOW-COVERED MOUNTAIN", "WASTES"
        /// </summary>
        public static readonly ReadOnlySet<string> BasicLandCardNames = new ReadOnlySet<string>(new HashSet<string>(new[] { "ISLAND", "PLAINS", "SWAMP", "FOREST", "MOUNTAIN", "SNOW-COVERED ISLAND", "SNOW-COVERED PLAINS", "SNOW-COVERED SWAMP", "SNOW-COVERED FOREST", "SNOW-COVERED MOUNTAIN", "WASTES" }, StringComparer.OrdinalIgnoreCase));

        /// <summary>
        /// A collection of all single values of the possible <see cref="BasicType"/>s.
        /// </summary>
        public static readonly ReadOnlyCollection<BasicType> AllBasicCardTypes = new ReadOnlyCollection<BasicType>(EnumFlagHelpers<BasicType>.ListAllSingleValues().ToList());

        /// <summary>
        /// A Set of all single values of the possible <see cref="BasicType"/>s.
        /// A readonly <see cref="ReadOnlySet{String}"/> wrapped around a <see cref="HashSet{String}"/> for fast lookup.
        /// </summary>
        public static readonly ReadOnlySet<BasicType> AllBasicCardTypesSet = new ReadOnlySet<BasicType>(AllBasicCardTypes);

#pragma warning disable 1591
        public const string Artifact = "Artifact";
        public const string Creature = "Creature";
        public const string BasicLand = "Basic Land";
        public const string Instant = "Instant";
        public const string Enchantment = "Enchantment";
        public const string Planeswalker = "Planeswalker";
        public const string Sorcery = "Sorcery";
        public const string Land = "Land";
#pragma warning restore 1591

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