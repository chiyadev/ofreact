using System.Collections.Generic;
using System.Linq;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a collection of objects.
    /// </summary>
    public class CollectionPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (!CollectionPropProvider.IsCollection(prop.Type, out _, out var itemType))
                return null;

            IEnumerable<YamlNode> items;

            switch (node)
            {
                case YamlSequenceNode n:
                    items = n;
                    break;

                case YamlScalarNode n when string.IsNullOrEmpty(n.Value):
                    items = Enumerable.Empty<YamlNode>();
                    break;

                default:
                    items = new[] { node };
                    break;
            }

            return new CollectionPropProvider(prop.Type, items.Select(n => ((IYamlComponentBuilder) context.Builder).PropResolver.Resolve(context, element, itemType, n)));
        }
    }
}