using System.Linq;
using System.Reflection;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that takes an element or a collection of elements.
    /// </summary>
    public class ChildrenPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            if (parameter == null)
                return null;

            var type    = parameter.ParameterType;
            var builder = (IYamlComponentBuilder) context.Builder;

            if (typeof(ofElement).IsAssignableFrom(type))
                return builder.BuildElement(context, node);

            if (CollectionPropProvider.IsCollection(type, out _, out var elementType) && typeof(ofElement).IsAssignableFrom(elementType))
                return node switch
                {
                    YamlSequenceNode sequence => new CollectionPropProvider(type, sequence.Select(n => builder.BuildElement(context, n))),
                    _                         => new CollectionPropProvider(type, new[] { builder.BuildElement(context, node) })
                };

            return null;
        }
    }
}