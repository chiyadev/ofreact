using System.Linq.Expressions;
using ofreact;
using osu.Framework.Localisation;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a <see cref="LocalisedString"/>.
    /// </summary>
    public class LocalizedStringPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (prop.Type != typeof(LocalisedString))
                return null;

            return new Provider(node.ToScalar().Value);
        }

        sealed class Provider : IPropProvider
        {
            readonly string _value;

            public Provider(string value)
            {
                _value = value;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Convert(Expression.Constant(_value), typeof(LocalisedString));
        }
    }
}