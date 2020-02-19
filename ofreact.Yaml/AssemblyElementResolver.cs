using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ofreact.Yaml
{
    public class AssemblyElementResolver : IElementTypeResolver
    {
        readonly Dictionary<string, Type> _types;

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

            _types = types.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ofElement)))
                          .ToDictionary(t => t.Name);
        }

        public Type Resolve(IYamlComponentBuilder builder, string name) => _types.GetValueOrDefault(name);
    }
}