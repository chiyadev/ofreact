using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Handles the type import part of a component.
    /// </summary>
    /// <remarks>
    /// Import is a sequence of scalars that are names or paths of the assemblies to load (using <see cref="Assembly.LoadFrom(string)"/>).
    /// Element types loaded from the assemblies will become available in the document for resolution.
    /// </remarks>
    public class ImportPartHandler : IComponentPartHandler
    {
        public bool Handle(ComponentBuilderContext context, string name, YamlNode node)
        {
            if (name != "import")
                return false;

            switch (node)
            {
                case YamlSequenceNode sequence:
                    var assemblies = new List<Assembly>();

                    foreach (var item in sequence)
                    {
                        if (!(item is YamlScalarNode scalar))
                            throw new YamlComponentException("Must be a scalar.", item);

                        if (string.IsNullOrEmpty(scalar.Value))
                            continue;

                        assemblies.Add(Assembly.LoadFrom(scalar.Value));
                    }

                    ((IYamlComponentBuilder) context.Builder).ElementResolver = new CompositeElementResolver(assemblies.Select(a => new AssemblyElementResolver(a)));
                    return true;

                case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                    return true;
            }

            return false;
        }
    }
}