using System;
using System.Globalization;
using System.Linq.Expressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a primitive type or string.
    /// </summary>
    public class PrimitivePropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementBuilder element, YamlNode node)
        {
            if (!prop.Type.IsPrimitive && prop.Type != typeof(string))
                return null;

            var value = node.ToScalar().Value;

            try
            {
                return new Provider(Convert.ChangeType(value, prop.Type, CultureInfo.InvariantCulture));
            }
            catch (Exception e)
            {
                throw new YamlComponentException($"Cannot convert '{value}' to type {prop.Type}.", node, e);
            }
        }

        sealed class Provider : IPropProvider
        {
            readonly object _value;

            public Provider(object value)
            {
                _value = value;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Constant(_value);
        }
    }
}