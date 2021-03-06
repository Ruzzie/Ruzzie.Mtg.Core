using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ruzzie.Mtg.Core
{
    /// <summary>
    /// Helper for Enums with the Flags type.
    /// </summary>
    /// <typeparam name="T">The Enum type</typeparam>
    public static class EnumFlagHelpers<T> where T : Enum, IConvertible, IComparable, IFormattable
    {
        private static readonly EnumTypeInfo TypeInfo;

        static EnumFlagHelpers()
        {
            TypeInfo = CreateEnumTypeInfo(typeof(T));
        }

        private static Func<T, T, T> GenerateBitwiseOr()
        {
            return BitwiseOperator(ExpressionType.Or);
        }

        private static Func<T, T, T> BitwiseOperator(ExpressionType expressionType)
        {
            //Thanks to: https://github.com/arogozine/EnumUtilities
            Type enumType = typeof(T);
            ParameterExpression leftVal = Expression.Parameter(enumType);
            ParameterExpression rightVal = Expression.Parameter(enumType);

            // Convert from Enum to Enum's underlying type (byte, int, long, ...)
            // to allow bitwise functions to work
            Type underlyingType = Enum.GetUnderlyingType(enumType);

            UnaryExpression leftValConverted = Expression.Convert(leftVal, underlyingType);
            UnaryExpression rightValConverted = Expression.Convert(rightVal, underlyingType);

            // left [expressionType] right
            BinaryExpression binaryExpression =
                Expression.MakeBinary(
                    expressionType,
                    leftValConverted,
                    rightValConverted);

            // Convert back to Enum
            UnaryExpression backToEnumType = Expression.Convert(binaryExpression, enumType);
            return Expression.Lambda<Func<T, T, T>>(backToEnumType, leftVal, rightVal).Compile();
        }

        private static HashSet<T> GenerateUniqueValues(Type enumType, Func<T, T, T> bitwiseOr)
        {
            Array values = Enum.GetValues(enumType);
            HashSet<T> uniqueValues = new HashSet<T>();

            for (var i = 0; i < values.Length; i++)
            {
                T singleValue = (T)values.GetValue(i);
                T combinedValue = singleValue;

                for (var j = values.Length - 1; j >= 0; j--)
                {                    
                    //each combination of 2
                    T secondValue = (T)values.GetValue(j);
                    uniqueValues.Add(bitwiseOr(singleValue, secondValue));

                    combinedValue = bitwiseOr(combinedValue, secondValue);
                    uniqueValues.Add(combinedValue);

                    T otherCombinedValue = secondValue;
                    for (int k = i+1; k < values.Length; k++)
                    {
                        T kValue = (T) values.GetValue(k);
                        otherCombinedValue = bitwiseOr(otherCombinedValue, kValue);
                        uniqueValues.Add(bitwiseOr(combinedValue, kValue));
                        uniqueValues.Add(otherCombinedValue);
						uniqueValues.Add(bitwiseOr(kValue, bitwiseOr(singleValue, secondValue)));
                    }
                }
            }
            return uniqueValues;
        }

        private static EnumTypeInfo CreateEnumTypeInfo(Type type)
        {
            EnumTypeInfo enumTypeInfo = new EnumTypeInfo {IsEnum = type.IsEnum(), HasFlagsAttribute = type.GetCustomAttributeForType(typeof(FlagsAttribute)) != null};

            if (enumTypeInfo.IsEnum)
            {
                enumTypeInfo.SingleValues = GenerateSingleValues(type);
                if (enumTypeInfo.HasFlagsAttribute)
                {
                    enumTypeInfo.BitwiseOr = GenerateBitwiseOr();
                    enumTypeInfo.UniqueValues = GenerateUniqueValues(type, enumTypeInfo.BitwiseOr);
                    enumTypeInfo.ValueWithAllFlagsSet =
                        GenerateValueWithAllFlagsSet(enumTypeInfo.SingleValues, enumTypeInfo.BitwiseOr);
                }
            }
         
            return enumTypeInfo;
        }

        private static T GenerateValueWithAllFlagsSet(IReadOnlyCollection<T> singleValues, Func<T, T, T> bitwiseOr)
        {
            T result = default(T);
            foreach (var singleFlagValue in singleValues)
            {
                result = bitwiseOr(result, singleFlagValue);
            }

            return result;
        }

        /// <summary>
        /// Gets the value with all flags set.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum
        /// or
        /// T does not have the Flags Attribute set
        /// </exception>
        public static T GetValueWithAllFlagsSet()
        {
            if (!TypeInfo.IsEnum)
            {
                throw new ArgumentException("T is not an enum");
            }

            if (!TypeInfo.HasFlagsAttribute)
            {
                throw new ArgumentException("T does not have the Flags Attribute set");
            }

            return TypeInfo.ValueWithAllFlagsSet;
        }

        /// <summary>
        /// Lists all possible unique flags combinations.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum
        /// or
        /// T does not have the Flags Attribute set
        /// </exception>
        public static IReadOnlyCollection<T> ListAllPossibleUniqueFlagsCombinations()
        {
            if (!TypeInfo.IsEnum)
            {
                throw new ArgumentException("T is not an enum");
            }

            if (!TypeInfo.HasFlagsAttribute)
            {
                throw new ArgumentException("T does not have the Flags Attribute set");
            }
            return TypeInfo.UniqueValues;
        }

        /// <summary>
        /// Lists all single values.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">T is not an enum</exception>
        public static IReadOnlyCollection<T> ListAllSingleValues()
        {
            if (!TypeInfo.IsEnum)
            {
                throw new ArgumentException("T is not an enum");
            }

            return TypeInfo.SingleValues;
        }

        private static IReadOnlyCollection<T> GenerateSingleValues(Type type)
        {
            HashSet<T> allSingleValues = new HashSet<T>();
            foreach (T value in Enum.GetValues(type))
            {
                allSingleValues.Add(value);
            }
           return allSingleValues;
        }

        private class EnumTypeInfo
        {
            public Func<T, T, T> BitwiseOr;
            public bool IsEnum { get; set; }
            public bool HasFlagsAttribute { get; set; }
            public IReadOnlyCollection<T> UniqueValues { get; set; }
            public IReadOnlyCollection<T> SingleValues { get; set; }
            public T ValueWithAllFlagsSet { get; set; }
        }
    }
}