using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ofreact
{
    static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ObjectsEqual(object[] a, object[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
            {
                var x = a[i];
                var y = b[i];

                if (x != y && (x == null || !x.Equals(y)))
                    return false;
            }

            return true;
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            do
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return field;
            }
            while ((type = type.BaseType) != null);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            do
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return method;
            }
            while ((type = type.BaseType) != null);
        }

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

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}