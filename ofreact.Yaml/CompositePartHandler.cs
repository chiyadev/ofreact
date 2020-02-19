using System.Linq;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class CompositePartHandler : IComponentPartHandler
    {
        readonly IComponentPartHandler[] _handlers;

        public CompositePartHandler(params IComponentPartHandler[] handlers)
        {
            _handlers = handlers;
        }

        public bool Handle(IYamlComponentBuilder builder, string name, YamlNode node) => _handlers.Any(h => h.Handle(builder, name, node));
    }
}