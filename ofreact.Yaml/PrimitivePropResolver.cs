using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a primitive type.
    /// This includes strings, built-in value types and enums.
    /// </summary>
    public class PrimitivePropResolver : IPropResolver
    {
        public bool IgnoreEnumCase { get; set; } = true;

        public IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            if (parameter == null)
                return null;

            return ResolveType(parameter.ParameterType, parameter.ParameterType, node);
        }

        static bool IsNullable(Type type, out Type underlyingType)
        {
            underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != null;
        }

        static string ParseNode(YamlNode node)
        {
            if (node is YamlScalarNode scalar)
                return scalar.Value;

            throw new YamlComponentException("Must be a scalar.", node);
        }

        IPropProvider ResolveType(Type exprType, Type type, YamlNode node)
        {
            // nullable
            if (IsNullable(type, out var underlyingType))
                return ResolveType(exprType, underlyingType, node);

            // string
            if (type == typeof(string))
                return new Provider(ParseNode(node), exprType);

            // primitive
            if (type.IsPrimitive)
            {
                var value = ParseNode(node);

                try
                {
                    return new Provider(Convert.ChangeType(value, type, CultureInfo.InvariantCulture), exprType);
                }
                catch (Exception e)
                {
                    throw new YamlComponentException($"Value '{value}' is not convertible to {type}.", node, e);
                }
            }

            // enum
            if (type.IsEnum)
            {
                var value = ParseNode(node);

                if (Enum.TryParse(type, value, IgnoreEnumCase, out var parsed))
                    return new Provider(parsed, exprType);

                throw new YamlComponentException($"Unknown enum value '{value}' in {type}.", node);
            }

            // collection
            if (CollectionPropProvider.IsCollection(type, out _, out var elementType))
                switch (node)
                {
                    case YamlSequenceNode sequence when sequence.Children.Count == 0:
                        return new CollectionPropProvider(type, Enumerable.Empty<IPropProvider>());

                    case YamlSequenceNode sequence:
                        if (ResolveType(elementType, elementType, sequence.Children[0]) == null)
                            break;

                        return new CollectionPropProvider(type, sequence.Select(n => ResolveType(elementType, elementType, n)));

                    case YamlScalarNode scalar:
                        var provider = ResolveType(elementType, elementType, scalar);

                        if (provider == null)
                            break;

                        return new CollectionPropProvider(type, new[] { provider });
                }

            return null;
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