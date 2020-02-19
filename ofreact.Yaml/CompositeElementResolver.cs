using System;
using System.Collections.Generic;
using System.Linq;

namespace ofreact.Yaml
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

        public Type Resolve(IYamlComponentBuilder builder, string name) => _resolvers.Select(r => r.Resolve(builder, name)).FirstOrDefault(t => t != null);
    }
}