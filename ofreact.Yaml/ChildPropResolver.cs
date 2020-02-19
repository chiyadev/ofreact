using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class ChildPropResolver : IPropResolver
    {
        public IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            var type = parameter.ParameterType;

            if (typeof(ofElement).IsAssignableFrom(type))
                return builder.BuildElement(node);

            if (CollectionPropProvider.IsCollection(type, out _))
                return node switch
                {
                    YamlSequenceNode sequence => new CollectionPropProvider(type, sequence.Select(builder.BuildElement)),
                    _                         => new CollectionPropProvider(type, new[] { builder.BuildElement(node) })
                };

            return null;
        }
    }
}