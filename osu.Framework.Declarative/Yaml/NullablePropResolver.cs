using System;
using System.Linq.Expressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves a nullable element prop by unwrapping the type.
    /// </summary>
    public class NullablePropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            var underlying = Nullable.GetUnderlyingType(prop.Type);

            if (underlying == null)
                return null;

            // rerun prop resolution using unwrapped nullable type
            return new Provider(prop.Type, ((IYamlComponentBuilder) context.Builder).PropResolver.Resolve(context, element, underlying, node));
        }

        sealed class Provider : IPropProvider
        {
            readonly Type _type;
            readonly IPropProvider _provider;

            public Provider(Type type, IPropProvider provider)
            {
                _type     = type;
                _provider = provider;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Convert(_provider.GetValue(context), _type); // this wraps value in nullable
        }
    }
}