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

        /// <summary>
        /// Finds a public or non-public instance field of the given type by the specified name from the full hierarchy.
        /// </summary>
        public static FieldInfo GetAllField(this Type type, string name)
        {
            do
            {
                var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (field != null)
                    return field;
            }
            while ((type = type.BaseType) != null);

            return null;
        }

        /// <summary>
        /// Finds a public or non-public instance method of the given type by the specified name from the full hierarchy.
        /// </summary>
        public static MethodInfo GetAllMethod(this Type type, string name)
        {
            do
            {
                var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (method != null)
                    return method;
            }
            while ((type = type.BaseType) != null);

            return null;
        }
    }
}