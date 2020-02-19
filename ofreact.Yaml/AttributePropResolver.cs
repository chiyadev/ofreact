using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class AttributePropResolver : IPropResolver
    {
        public IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            IPropProvider resolve(IEnumerable<Attribute> attrs)
                => attrs.OfType<IPropResolver>().Select(r => r.Resolve(builder, element, parameter, node)).FirstOrDefault(p => p != null);

            return resolve(parameter.GetCustomAttributes()) ?? resolve(parameter.ParameterType.GetCustomAttributes());
        }
    }
}