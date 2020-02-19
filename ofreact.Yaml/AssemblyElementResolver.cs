using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ofreact.Yaml
{
    /// <summary>
    /// Resolves an element type from all types defined in an assembly by their name or full name.
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
                if (type.IsAbstract || !typeof(ofElement).IsAssignableFrom(type))
                    continue;

                _types.TryAdd(type.Name, type);
                _types.TryAdd(type.FullName, type);
            }
        }

        public Type Resolve(IYamlComponentBuilder builder, string name) => _types.GetValueOrDefault(name);
    }
}