using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Ruzzie.Mtg.Core
{
    public static class EnumFlagHelpers<T> where T : struct, IConvertible, IComparable, IFormattable
    {
        private static readonly EnumTypeInfo TypeInfo;

        static EnumFlagHelpers()
        {
            TypeInfo = GetEnumTypeInfo(typeof(T));
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
                T currVal = (T) values.GetValue(i);
                uniqueValues.Add(currVal);

                for (var j = 0; j < values.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    currVal = bitwiseOr(currVal, (T) values.GetValue(j));
                    uniqueValues.Add(currVal);
                }
            }
            return uniqueValues;
        }

        private static EnumTypeInfo GetEnumTypeInfo(Type type)
        {
            EnumTypeInfo enumTypeInfo = new EnumTypeInfo {IsEnum = type.IsEnum, HasFlagsAttribute = type.GetCustomAttribute(typeof(FlagsAttribute)) != null};

            if (enumTypeInfo.IsEnum)
            {
                enumTypeInfo.SingleValues = GenerateSingleValues(type);
                if (enumTypeInfo.HasFlagsAttribute)
                {
                    enumTypeInfo.BitwiseOr = GenerateBitwiseOr();
                    enumTypeInfo.UniqueValues = GenerateUniqueValues(type, enumTypeInfo.BitwiseOr);
                }
            }
         
            return enumTypeInfo;
        }

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
        }
    }
}