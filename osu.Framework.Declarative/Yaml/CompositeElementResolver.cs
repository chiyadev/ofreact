using System;
using System.Collections.Generic;
using System.Linq;
using ofreact;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element type from a collection of nested resolvers.
    /// </summary>
    public class CompositeElementResolver : IElementTypeResolver
    {
        readonly IElementTypeResolver[] _resolvers;

        public CompositeElementResolver(IEnumerable<IElementTypeResolver> resolvers) : this(resolvers.ToArray()) { }

        public CompositeElementResolver(params IElementTypeResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public Type Resolve(ComponentBuilderContext context, string name) => _resolvers.Select(r => r.Resolve(context, name)).FirstOrDefault(t => t != null);
    }
}