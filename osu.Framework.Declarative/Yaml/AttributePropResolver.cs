using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop from attributes that implement <see cref="IPropResolver"/>.
    /// </summary>
    /// <remarks>
    /// Attribute resolvers should be stateless.
    /// This class will cache all attribute resolvers encountered during its lifetime.
    /// </remarks>
    public class AttributePropResolver : IPropResolver
    {
        readonly ConcurrentDictionary<object, IPropResolver[]> _resolvers = new ConcurrentDictionary<object, IPropResolver[]>();

        public IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            if (parameter == null)
                return null;

            return resolve(parameter, parameter.GetCustomAttributes) ?? resolve(parameter.ParameterType, parameter.ParameterType.GetCustomAttributes);

            IPropProvider resolve(object key, Func<IEnumerable<Attribute>> attrs)
            {
                if (!_resolvers.TryGetValue(key, out var resolvers))
                    _resolvers[key] = resolvers = attrs().OfType<IPropResolver>().ToArray();

                return resolvers.Select(r => r.Resolve(context, name, element, parameter, node)).FirstOrDefault(p => p != null);
            }
        }
    }
}