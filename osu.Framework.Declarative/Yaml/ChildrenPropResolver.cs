using System.Linq;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that takes an element or a collection of elements.
    /// </summary>
    public class ChildrenPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementRenderInfo element, YamlNode node)
        {
            if (prop.Type == null)
                return null;

            var builder = (IYamlComponentBuilder) context.Builder;

            if (typeof(ofElement).IsAssignableFrom(prop.Type))
                return builder.BuildElement(context, node);

            if (CollectionPropProvider.IsCollection(prop.Type, out _, out var elementType) && typeof(ofElement).IsAssignableFrom(elementType))
                return node switch
                {
                    YamlSequenceNode sequence => new CollectionPropProvider(prop.Type, sequence.Select(n => builder.BuildElement(context, n))),
                    _                         => new CollectionPropProvider(prop.Type, new[] { builder.BuildElement(context, node) })
                };

            return null;
        }
    }
}