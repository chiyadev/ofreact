using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that takes a nested element.
    /// </summary>
    public class ChildPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (!typeof(ofElement).IsAssignableFrom(prop.Type))
                return null;

            return ((IYamlComponentBuilder) context.Builder).BuildElement(context, node);
        }
    }
}