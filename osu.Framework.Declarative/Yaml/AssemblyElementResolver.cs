using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ofreact;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element type from all public types defined in an assembly by their name or full name.
    /// </summary>
    public class AssemblyElementResolver : IElementTypeResolver
    {
        readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();

        public AssemblyElementResolver(Assembly assembly)
        {
            IEnumerable<Type> types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            foreach (var type in types.OrderBy(t => t.FullName))
            {
                var name = type.FullName;

                if (name == null || !type.IsPublic || type.IsAbstract || !typeof(ofElement).IsAssignableFrom(type))
                    continue;

                var start = name.LastIndexOf('.') + 1;

                name = name.Replace('+', '.'); // for nested types

                _types.TryAdd(name, type);
                _types.TryAdd(name.Substring(start), type);
            }
        }

        public Type Resolve(ComponentBuilderContext context, string name) => _types.GetValueOrDefault(name);
    }
}