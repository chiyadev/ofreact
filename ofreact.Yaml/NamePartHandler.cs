using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    /// <summary>
    /// Handles the name of a component.
    /// </summary>
    public class NamePartHandler : IComponentPartHandler
    {
        public bool Handle(ComponentBuilderContext context, string name, YamlNode node)
        {
            if (name != "name")
                return false;

            if (!(node is YamlScalarNode scalar))
                throw new YamlComponentException("Must be a scalar.", node);

            context.Name = scalar.Value;
            return true;
        }
    }
}