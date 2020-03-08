using System.Collections.Generic;
using System.Linq;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop from a collection of nested resolvers.
    /// </summary>
    public class CompositePropResolver : IPropResolver
    {
        readonly IPropResolver[] _resolvers;

        public CompositePropResolver(IEnumerable<IPropResolver> resolvers) : this(resolvers.ToArray()) { }

        public CompositePropResolver(params IPropResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
            => _resolvers.Select(r => r.Resolve(context, element, prop, node)).FirstOrDefault(p => p != null);

        public bool Resolve(ComponentBuilderContext context, ElementBuilder element, string prop, YamlNode node)
            => _resolvers.Any(r => r.Resolve(context, element, prop, node));
    }
}