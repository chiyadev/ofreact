using System;
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
    public class AttributePropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementRenderInfo element, YamlNode node)
        {
            return resolve(prop.GetCustomAttributes()) ?? resolve(prop.Type.GetCustomAttributes());

            IPropProvider resolve(IEnumerable<Attribute> attrs)
                => attrs.OfType<IPropResolver>()
                        .Select(r => r.Resolve(context, prop, element, node))
                        .FirstOrDefault(p => p != null);
        }
    }
}