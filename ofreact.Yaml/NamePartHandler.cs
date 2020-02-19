using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class NamePartHandler : IComponentPartHandler
    {
        public bool Handle(IYamlComponentBuilder builder, string name, YamlNode node)
        {
            if (name != "name")
                return false;

            if (node is YamlScalarNode scalar)
            {
                builder.Name = scalar.Value;

                return true;
            }

            throw new YamlComponentException("Must be a scalar.", node);
        }
    }
}