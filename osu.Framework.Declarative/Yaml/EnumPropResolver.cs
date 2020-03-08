using System;
using System.Linq.Expressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is an enum.
    /// </summary>
    public class EnumPropResolver : IPropResolver
    {
        public bool IgnoreEnumCase { get; set; } = true;

        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (!prop.Type.IsEnum)
                return null;

            var value = node.ToScalar().Value;

            if (Enum.TryParse(prop.Type, value, IgnoreEnumCase, out var parsed))
                return new Provider(parsed, prop.Type);

            throw new YamlComponentException($"Cannot convert '{value}' to enum {prop.Type}.", node);
        }

        sealed class Provider : IPropProvider
        {
            readonly object _value;
            readonly Type _type;

            public Provider(object value, Type type)
            {
                _value = value;
                _type  = type;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Constant(_value, _type);
        }
    }
}