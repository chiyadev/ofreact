using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
            => _resolvers.Select(r => r.Resolve(context, name, element, parameter, node)).FirstOrDefault(p => p != null);
    }
}