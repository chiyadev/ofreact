using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    /// <summary>
    /// Resolves an element prop from a collection of nested resolvers.
    /// </summary>
    public class CompositePropResolver : IPropResolver
    {
        readonly IPropResolver[] _resolvers;

        public CompositePropResolver(params IPropResolver[] resolvers)
        {
            _resolvers = resolvers;
        }

        public IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
            => _resolvers.Select(r => r.Resolve(builder, element, parameter, node)).FirstOrDefault(p => p != null);
    }
}