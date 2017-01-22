using System;
using System.Reflection;

namespace Ruzzie.Mtg.Core
{
#if HAVE_FULL_REFLECTION
   internal static class TypeExtensions
    {
        public static Assembly Assembly(this Type type)
        {
            return type.Assembly;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }
        
        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        public static Type BaseType(this Type type)
        {
            return type.BaseType;
        }
        public static Attribute GetCustomAttributeForType(this Type type, Type attributeType)
        {
            return type.GetCustomAttribute(attributeType);
        }
    }
#else
    internal static class TypeExtensions
    {
        public static Assembly Assembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }

        public static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static Attribute GetCustomAttributeForType(this Type type , Type attributeType )
        {
            return type.GetTypeInfo().GetCustomAttribute(attributeType);
        }
    }
#endif
}