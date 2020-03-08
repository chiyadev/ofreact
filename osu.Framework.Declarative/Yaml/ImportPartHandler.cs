using System;
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

            var assemblies = new List<Assembly>();

            foreach (var item in node.ToSequence())
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(item.ToScalar().Value));
                }
                catch (Exception e)
                {
                    context.OnException(e);
                }
            }

            // append as element resolver with new assemblies
            var builder = (IYamlComponentBuilder) context.Builder;

            builder.ElementResolver = new CompositeElementResolver(assemblies.Select(a => new AssemblyElementResolver(a)).Append(builder.ElementResolver));

            return true;
        }
    }
}