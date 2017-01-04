using System;
using System.Collections.Concurrent;

namespace Ruzzie.Mtg.Core
{
    public static class BasicTypes
    {
        private static readonly ConcurrentDictionary<string,BasicType> EnumNameCache = new ConcurrentDictionary<string, BasicType>(StringComparer.OrdinalIgnoreCase);
        
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
            if (StringComparer.OrdinalIgnoreCase.Equals(currentTypeString.Trim(), "Basic Land"))
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