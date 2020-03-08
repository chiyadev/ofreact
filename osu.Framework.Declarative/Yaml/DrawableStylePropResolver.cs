using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osu.Framework.Graphics;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that is a <see cref="DrawableStyleDelegate{T}"/> or any matching property of <see cref="Drawable"/> that can be appended to such delegate.
    /// </summary>
    public class DrawableStylePropResolver : IPropResolver
    {
        static bool IsStyleDelegate(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DrawableStyleDelegate<>);

        // resolving a style delegate parameter
        public IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node)
        {
            if (!IsStyleDelegate(prop.Type))
                return null;

            var provider = new Provider(prop.Type);

            foreach (var (key, value) in node.ToMapping())
            {
                try
                {
                    // find member by name
                    var memberName = key.ToScalar().Value;
                    var member     = provider.FindMember(memberName);

                    // resolve member using prop provider
                    var memberProvider = member == null ? null : ((IYamlComponentBuilder) context.Builder).PropResolver.Resolve(context, null, member, value);

                    if (memberProvider == null)
                        throw new YamlComponentException($"Cannot resolve property or field '{memberName}' in element {provider.DrawableType}.", key);

                    provider.Properties[member] = memberProvider;
                }
                catch (Exception e)
                {
                    context.OnException(e);
                }
            }

            return provider;
        }

        // resolving some prop that could be a property of drawable
        // the assignment gets appended to the style delegate
        public bool Resolve(ComponentBuilderContext context, ElementBuilder element, string prop, YamlNode node)
        {
            if (element == null)
                return false;

            // find style delegate parameter
            var styleProps = element.Parameters.Where(p => IsStyleDelegate(p.ParameterType)).ToArray();

            if (styleProps.Length == 0)
                return false;

            if (styleProps.Length != 1)
                throw new YamlComponentException($"Ambiguous element style prop reference: {string.Join<ParameterInfo>(", ", styleProps)}", node);

            var styleProp = styleProps[0];

            // add or create style delegate prop provider
            Provider provider;

            if (element.Props.TryGetValue(styleProp.Name, out var p) && p is Provider pp)
                provider = pp;

            else
                element.Props[styleProp.Name] = provider = new Provider(styleProp.ParameterType);

            // find member by name
            var member = provider.FindMember(prop);

            if (member == null)
                return false;

            // resolve member using prop provider
            var memberProvider = ((IYamlComponentBuilder) context.Builder).PropResolver.Resolve(context, element, member, node);

            if (memberProvider == null)
                throw new YamlComponentException($"Cannot resolve property or field '{prop}' in element {provider.DrawableType}.", node);

            provider.Properties[member] = memberProvider;

            return true;
        }

        sealed class Provider : IPropProvider
        {
            public readonly Type DelegateType;
            public readonly Type DrawableType;

            public Provider(Type delegateType)
            {
                DelegateType = delegateType;
                DrawableType = delegateType.GetGenericArguments()[0];
            }

            public readonly Dictionary<MemberInfo, IPropProvider> Properties = new Dictionary<MemberInfo, IPropProvider>();

            public MemberInfo FindMember(string name)
            {
                const BindingFlags binding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

                // find writable property
                var property = DrawableType.GetProperty(name, binding);

                if (property != null && property.CanWrite)
                    return property;

                // find field
                return DrawableType.GetField(name, binding);
            }

            public Expression GetValue(ComponentBuilderContext context)
            {
                var drawable = Expression.Parameter(DrawableType, "drawable");

                return Expression.Lambda(
                    DelegateType,
                    Expression.Block(Properties.Select(x =>
                        Expression.Assign(
                            Expression.MakeMemberAccess(drawable, x.Key),
                            x.Value.GetValue(context)))),
                    drawable);
            }
        }
    }
}