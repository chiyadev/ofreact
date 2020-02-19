using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class PrimitivePropResolver : IPropResolver
    {
        public bool IgnoreEnumCase { get; set; } = true;

        public IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            var type = parameter.ParameterType;

            if (type == typeof(string))
                return new Provider(ParseNode(node), type);

            if (type.IsPrimitive)
            {
                var value = ParseNode(node);

                try
                {
                    return new Provider(Convert.ChangeType(value, type, CultureInfo.InvariantCulture), type);
                }
                catch (Exception e)
                {
                    throw new YamlComponentException($"Value '{value}' is not convertible to {type}.", node, e);
                }
            }

            if (type.IsEnum)
            {
                var value = ParseNode(node);

                if (Enum.TryParse(type, value, IgnoreEnumCase, out var parsed))
                    return new Provider(parsed, type);

                throw new YamlComponentException($"Unknown enum value '{value}' in {type}.", node);
            }

            return null;
        }

        static string ParseNode(YamlNode node)
        {
            if (node is YamlScalarNode scalar)
                return scalar.Value;

            throw new YamlComponentException("Must be a scalar.", node);
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

            public Expression GetValue(Expression node) => Expression.Constant(_value, _type);
        }
    }
}