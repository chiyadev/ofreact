using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace ofreact
{
    /// <summary>
    /// Used to compare the props of two objects for equality.
    /// </summary>
    public static class PropComparer
    {
        static readonly ConcurrentDictionary<Type, Func<object, object, bool>> _comparer = new ConcurrentDictionary<Type, Func<object, object, bool>>();

        /// <summary>
        /// Returns true if all fields of object <paramref name="a"/> and <paramref name="b"/> marked with <see cref="PropAttribute"/> are equal.
        /// </summary>
        public static bool Equal(object a, object b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            var type = a.GetType();

            if (type != b.GetType())
                return false;

            if (_comparer.TryGetValue(type, out var comparer))
                return comparer(a, b);

            // parameters: a, b
            var pa = Expression.Parameter(typeof(object), "a");
            var pb = Expression.Parameter(typeof(object), "b");

            // comparison variables: x, y (casted parameters)
            var x = Expression.Variable(type, "x");
            var y = Expression.Variable(type, "y");

            comparer = Expression.Lambda<Func<object, object, bool>>(
                                      Expression.Block(new[] { x, y },
                                          Expression.Assign(x, Expression.Convert(pa, type)),
                                          Expression.Assign(y, Expression.Convert(pb, type)),
                                          Build(type, x, y)),
                                      pa, pb)
                                 .CompileSafe();

            return (_comparer[type] = comparer)(a, b);
        }

        public static Expression Build(Type type, Expression a, Expression b)
        {
            var expr = null as Expression;

            foreach (var field in type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)))
            {
                var also = Expression.Equal(Expression.Field(a, field), Expression.Field(b, field));

                expr = expr == null
                    ? also
                    : Expression.AndAlso(expr, also);
            }

            return expr ?? Expression.Constant(true);
        }
    }
}