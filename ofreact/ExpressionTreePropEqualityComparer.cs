using System;
using System.Linq;
using System.Linq.Expressions;

namespace ofreact
{
    /// <summary>
    /// Prop comparer that uses expression trees.
    /// </summary>
    public class ExpressionTreePropEqualityComparer : IPropEqualityComparer
    {
        public Func<ofElement, ofElement, bool> Equals { get; }

        public ExpressionTreePropEqualityComparer(Type type)
        {
            var a = Expression.Parameter(type, "a");
            var b = Expression.Parameter(type, "b");

            var body = null as Expression;

            foreach (var field in type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)))
            {
                var other = Expression.Equal(
                    Expression.Field(a, field),
                    Expression.Field(b, field));

                body = body == null
                    ? other
                    : Expression.AndAlso(body, other);
            }

            if (body == null)
            {
                Equals = (x, y) => true;
            }
            else
            {
                var x = Expression.Parameter(typeof(ofElement), "x");
                var y = Expression.Parameter(typeof(ofElement), "y");

                Equals = Expression.Lambda<Func<ofElement, ofElement, bool>>(
                                        Expression.Invoke(
                                            Expression.Lambda(body, a, b),
                                            Expression.Convert(x, type),
                                            Expression.Convert(y, type)),
                                        x, y)
                                   .Compile();
            }
        }
    }
}