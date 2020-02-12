using System;
using System.Collections.Generic;
using System.Reflection;

namespace ofreact
{
    static class ReflectionHelper
    {
        /// <summary>
        /// Enumerates all public and non-public instance fields of the given type from the full hierarchy.
        /// </summary>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            do
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return field;
            }
            while ((type = type.BaseType) != null);
        }

        /// <summary>
        /// Enumerates all public and non-public instance methods of the given type from the full hierarchy.
        /// </summary>
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            do
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return method;
            }
            while ((type = type.BaseType) != null);
        }
    }
}