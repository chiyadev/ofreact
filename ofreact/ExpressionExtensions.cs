using System;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;

namespace ofreact
{
    /// <summary>
    /// Contains extension methods for expression trees.
    /// </summary>
    public static class ExpressionExtensions
    {
        static readonly bool _fast;

        static ExpressionExtensions()
        {
            try
            {
                // this should fail with aot compiler
                _fast = Expression.Lambda<Func<bool>>(Expression.Constant(true)).CompileFast()();
            }
            catch
            {
                _fast = false;
            }
        }

        /// <inheritdoc cref="Expression{T}.Compile()"/>
        public static TDelegate CompileSafe<TDelegate>(this Expression<TDelegate> expression) where TDelegate : class
        {
            try
            {
                if (_fast)
                    return expression.CompileFast();
            }
            catch
            {
                // ignored
            }

            return expression.Compile();
        }

        static readonly MethodInfo _fieldInfoSetValueMethod = typeof(FieldInfo).GetMethod(nameof(FieldInfo.SetValue), new[] { typeof(object), typeof(object) });

        /// <summary>
        /// Creates an <see cref="Expression"/> that represents an assignment operation.
        /// </summary>
        /// <remarks>
        /// If <paramref name="left"/> is a field expression and the field is readonly, this method will use reflection to set the field value.
        /// </remarks>
        public static Expression AssignSafe(Expression left, Expression right)
        {
            if (left is MemberExpression member && member.Member is FieldInfo field && field.IsInitOnly)
                return Expression.Call(Expression.Constant(field), _fieldInfoSetValueMethod, member.Expression, right);

            return Expression.Assign(left, right);
        }
    }
}