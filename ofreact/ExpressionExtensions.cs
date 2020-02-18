using System;
using System.Linq.Expressions;
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
            => _fast
                ? expression.CompileFast()
                : expression.Compile();
    }
}