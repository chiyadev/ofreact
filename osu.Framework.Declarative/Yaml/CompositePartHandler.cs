using System.Collections.Generic;
using System.Linq;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Handles a part of a component using a collection of nested handlers.
    /// </summary>
    public class CompositePartHandler : IComponentPartHandler
    {
        readonly IComponentPartHandler[] _handlers;

        public CompositePartHandler(IEnumerable<IComponentPartHandler> resolvers) : this(resolvers.ToArray()) { }

        public CompositePartHandler(params IComponentPartHandler[] handlers)
        {
            _handlers = handlers;
        }

        public bool Handle(ComponentBuilderContext context, string name, YamlNode node) => _handlers.Any(h => h.Handle(context, name, node));
    }
}