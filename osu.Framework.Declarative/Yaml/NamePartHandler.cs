using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
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

            context.Name = node.ToScalar().Value;
            return true;
        }
    }
}