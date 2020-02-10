using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    public static class PropEquality
    {
        public delegate bool Comparer(ofElement a, ofElement b);

        static readonly ConcurrentDictionary<Type, Comparer> _comparer = new ConcurrentDictionary<Type, Comparer>();

        /// <summary>
        /// Compares the two given <see cref="ofElement"/> by their fields annotated with <see cref="PropAttribute"/>.
        /// </summary>
        /// <returns>True if all props from both elements are equal.</returns>
        public static bool AreEqual(ofElement a, ofElement b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            var type = a.GetType();

            if (type != b.GetType())
                return false;

            return GetComparer(type)(a, b);
        }

        public static Comparer GetComparer(Type type)
        {
            if (_comparer.TryGetValue(type, out var comparer))
                return comparer;

            return _comparer[type] = BuildComparer(type);
        }

        // use expression trees by default
        public static Comparer BuildComparer(Type type) => BuildExpressionTreeComparer(type);

        public static Comparer BuildExpressionTreeComparer(Type type)
        {
            var fields = new Queue<FieldInfo>(type.GetFields().Where(f => f.IsDefined(typeof(PropAttribute), true)));

            var a = Expression.Parameter(type, "a");
            var b = Expression.Parameter(type, "b");

            var expr = null as Expression;

            while (fields.TryDequeue(out var field))
            {
                var other = Expression.Equal(Expression.Field(a, field), Expression.Field(b, field));

                expr = expr == null
                    ? other
                    : Expression.AndAlso(expr, other);
            }

            if (expr == null)
                return null;

            var x = Expression.Parameter(typeof(ofElement), "a");
            var y = Expression.Parameter(typeof(ofElement), "b");

            return Expression.Lambda<Comparer>(
                                  Expression.Invoke(
                                      Expression.Lambda(expr, a, b),
                                      Expression.Convert(x, type),
                                      Expression.Convert(y, type)),
                                  x, y)
                             .Compile();
        }
    }
}