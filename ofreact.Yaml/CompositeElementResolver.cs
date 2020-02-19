using System;
using System.Linq;

namespace ofreact.Yaml
{
    public class CompositeElementResolver : IElementTypeResolver
    {
        readonly IElementTypeResolver[] _resolvers;

        public CompositeElementResolver(params IElementTypeResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public Type Resolve(IYamlComponentBuilder builder, string name) => _resolvers.Select(r => r.Resolve(builder, name)).FirstOrDefault(t => t != null);
    }
}