using System;
using System.Collections.Generic;
using System.Linq;

namespace ofreact
{
    /// <summary>
    /// Defines useful extension methods that aids development with ofreact.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Projects each element of a sequence into an <see cref="ofElement"/> and returns a fragment that renders them.
        /// </summary>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <returns>An <see cref="ofFragment"/> that renders the projected elements.</returns>
        public static ofFragment Map<TSource>(this IEnumerable<TSource> source, Func<TSource, ofElement> selector)
            => source.Select(selector)
                     .ToArray();

        /// <inheritdoc cref="Map{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,ofreact.ofElement})"/>
        public static ofFragment Map<TSource>(this IEnumerable<TSource> source, Func<TSource, int, ofElement> selector)
            => source.Select(selector)
                     .ToArray();
    }
}