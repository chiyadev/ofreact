using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using AgileObjects.ReadableExpressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Used to handle one key-value mapping in the document root.
    /// </summary>
    public interface IComponentPartHandler
    {
        /// <param name="context">Builder context.</param>
        /// <param name="name">Name of the part.</param>
        /// <param name="node">YAML node of the part.</param>
        bool Handle(ComponentBuilderContext context, string name, YamlNode node);
    }

    /// <summary>
    /// Used to find an element type from a string.
    /// </summary>
    public interface IElementTypeResolver
    {
        /// <param name="context">Builder context.</param>
        /// <param name="name">Name of the element to resolve to the type.</param>
        Type Resolve(ComponentBuilderContext context, string name);
    }

    /// <summary>
    /// Used to create a prop provider from an YAML node.
    /// </summary>
    public interface IPropResolver
    {
        /// <summary>
        /// Resolves the given prop.
        /// </summary>
        /// <param name="context">Builder context.</param>
        /// <param name="element">Element containing this prop that is currently being built.
        /// This can be null if the prop being resolved is not directly related to the element being built..</param>
        /// <param name="prop">Type information of the prop.</param>
        /// <param name="node">YAML node of the prop.</param>
        IPropProvider Resolve(ComponentBuilderContext context, ElementBuilder element, PropTypeInfo prop, YamlNode node);

        /// <summary>
        /// Resolves the given prop.
        /// </summary>
        /// <remarks>
        /// This is only called to resolve excessive props that are not bound to any parameter.
        /// </remarks>
        /// <param name="context">Builder context.</param>
        /// <param name="element">Element containing this prop that is currently being built.
        /// This can be null if the prop being resolved is not directly related to the element being built.</param>
        /// <param name="prop">Name of the prop.</param>
        /// <param name="node">YAML node of the prop.</param>
        bool Resolve(ComponentBuilderContext context, ElementBuilder element, string prop, YamlNode node) => false;
    }

    /// <summary>
    /// Contains type information of a prop.
    /// </summary>
    /// <remarks>
    /// This struct can represent a constructor parameter, member property or field, or a type.
    /// </remarks>
    public struct PropTypeInfo
    {
        readonly ParameterInfo _parameter;
        readonly MemberInfo _member;

        PropTypeInfo(ParameterInfo parameter, MemberInfo member)
        {
            _parameter = parameter;
            _member    = member;
        }

        /// <summary>
        /// Name of the prop.
        /// </summary>
        public string Name => Parameter?.Name ?? _member.Name;

        public ParameterInfo Parameter => _parameter;
        public PropertyInfo Property => _member as PropertyInfo;
        public FieldInfo Field => _member as FieldInfo;

        /// <summary>
        /// Type of the prop.
        /// </summary>
        public Type Type => Parameter?.ParameterType
                         ?? Property?.PropertyType
                         ?? Field?.FieldType
                         ?? _member as Type;

        /// <summary>
        /// Enumerates all attributes applied on this prop.
        /// </summary>
        public IEnumerable<Attribute> GetAttributes()
            => Parameter?.GetCustomAttributes().Concat(Parameter.ParameterType.GetCustomAttributes())
            ?? Property?.GetCustomAttributes().Concat(Property.PropertyType.GetCustomAttributes())
            ?? Field?.GetCustomAttributes().Concat(Field.FieldType.GetCustomAttributes())
            ?? Type?.GetCustomAttributes()
            ?? Enumerable.Empty<Attribute>();

        public static implicit operator PropTypeInfo(ParameterInfo parameter) => new PropTypeInfo(parameter, null);
        public static implicit operator PropTypeInfo(MemberInfo member) => new PropTypeInfo(null, member);
    }

    public interface IYamlComponentBuilder : IComponentBuilder
    {
        IComponentPartHandler PartHandler { get; set; }
        IElementTypeResolver ElementResolver { get; set; }
        IPropResolver PropResolver { get; set; }

        ElementBuilder BuildElement(ComponentBuilderContext context, YamlNode node);

        /// <summary>
        /// Builds a C# source code of the rendering function.
        /// </summary>
        string GenerateSource();
    }

    /// <summary>
    /// Transforms a <see cref="YamlDocument"/> to an <see cref="ofElement"/>.
    /// </summary>
    public class YamlComponentBuilder : ComponentBuilderBase, IYamlComponentBuilder
    {
        readonly YamlMappingNode _mapping;

        /// <summary>
        /// Default <see cref="PartHandler"/> for all instances of <see cref="YamlComponentBuilder"/>.
        /// </summary>
        public static IComponentPartHandler DefaultPartHandler { get; set; } =
            new CompositePartHandler(
                new NamePartHandler(),
                new ImportPartHandler());

        /// <summary>
        /// Default <see cref="ElementResolver"/> for all instances of <see cref="YamlComponentBuilder"/>.
        /// </summary>
        public static IElementTypeResolver DefaultTypeResolver { get; set; } =
            new PrefixedElementResolver("of",
                new CompositeElementResolver(
                    new AssemblyElementResolver(typeof(ofElement).Assembly),
                    new AssemblyElementResolver(typeof(YamlComponentBuilder).Assembly)));

        /// <summary>
        /// Default <see cref="PropResolver"/> for all instances of <see cref="YamlComponentBuilder"/>.
        /// </summary>
        public static IPropResolver DefaultPropResolver { get; set; } =
            new CompositePropResolver(
                new AttributePropResolver(),
                new NullablePropResolver(),
                new PrimitivePropResolver(),
                new EnumPropResolver(),
                new CollectionPropResolver(),
                new KeyPropResolver(),
                new ChildPropResolver(),
                new VectorPropResolver(),
                new ColorPropResolver(),
                new LocalizedStringPropResolver(),
                new FontUsagePropResolver(),
                new MarginPaddingPropResolver(),
                new DrawableStylePropResolver());

        public IComponentPartHandler PartHandler { get; set; } = DefaultPartHandler;
        public IElementTypeResolver ElementResolver { get; set; } = DefaultTypeResolver;
        public IPropResolver PropResolver { get; set; } = DefaultPropResolver;

        /// <summary>
        /// Creates a new <see cref="YamlComponentBuilder"/> with the given document to construct the component from.
        /// </summary>
        public YamlComponentBuilder(YamlDocument document)
        {
            if (document.RootNode is YamlMappingNode mapping)
                _mapping = mapping;

            else
                throw new ArgumentException("Document root must be a mapping.");
        }

        protected override ElementBuilder Render(ComponentBuilderContext context)
        {
            var render = null as YamlNode;

            foreach (var (keyNode, valueNode) in _mapping)
            {
                try
                {
                    var key = keyNode.ToScalar().Value;

                    if (key == "render")
                    {
                        render = valueNode;
                        continue;
                    }

                    if (!PartHandler.Handle(context, key, valueNode))
                        throw new YamlComponentException($"Invalid component part '{key}'.", keyNode);
                }
                catch (Exception e)
                {
                    context.OnException(e);
                }
            }

            if (render == null)
                return null;

            return BuildElement(context, render);
        }

        readonly struct YamlProp
        {
            public readonly YamlScalarNode Key;
            public readonly YamlNode Value;

            public YamlProp(YamlNode key, YamlNode value)
            {
                Key   = (YamlScalarNode) key;
                Value = value;
            }
        }

        public ElementBuilder BuildElement(ComponentBuilderContext context, YamlNode node)
        {
            try
            {
                switch (node)
                {
                    // element
                    case YamlMappingNode mapping:
                        if (mapping.Children.Count == 0)
                            return new ElementBuilder.Empty();

                        if (mapping.Children.Count != 1)
                            throw new YamlComponentException("Mapping must have one key that indicates element type.", mapping);

                        var (keyNode, valueNode) = mapping.Children.First();

                        var key = keyNode.ToScalar().Value;

                        // resolve type
                        var type = ElementResolver.Resolve(context, key) ?? throw new YamlComponentException($"Cannot resolve element '{key}'.", keyNode);

                        // build prop dictionary
                        var props = valueNode.ToMapping().ToDictionary(x => x.Key.ToScalar().Value, x => new YamlProp(x.Key, x.Value));

                        return BuildElementWithProps(context, type, keyNode, props);

                    // fragment
                    case YamlSequenceNode sequence:
                        var elements = sequence.Select(n => BuildElement(context, n)).Where(e => !(e is ElementBuilder.Empty)).ToArray();

                        return elements.Length switch
                        {
                            0 => new ElementBuilder.Empty(),
                            1 => elements[0],
                            _ => new FragmentBuilder(elements)
                        };

                    case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                        return new ElementBuilder.Empty();

                    default:
                        throw new YamlComponentException("Must be a mapping or sequence.", node);
                }
            }
            catch (Exception e)
            {
                context.OnException(e);

                return new ElementBuilder.Empty();
            }
        }

        public string GenerateSource() => BuildRendererExpression().ToReadableString();

        sealed class ElementMatch
        {
            public readonly ElementBuilder Element;
            public readonly Exception Exception;
            public readonly float Relevance;

            public ElementMatch(ElementBuilder element, List<Exception> exceptions, float points)
            {
                Element = element;

                if (exceptions.Count != 0)
                    Exception = new AggregateException(exceptions);

                // relevance is relative
                Relevance = points / element.Parameters.Length;
            }
        }

        ElementBuilder BuildElementWithProps(ComponentBuilderContext context, Type type, YamlNode node, IReadOnlyDictionary<string, YamlProp> props)
        {
            var matches = new List<ElementMatch>();

            foreach (var constructor in type.GetConstructors())
            {
                var element = new ElementBuilder(type, constructor);
                var points  = 0f;

                // we keep our own list of exceptions because we want to try out every element constructor and find the one that matches
                var exceptions = new List<Exception>();

                var matchedParams = new HashSet<string>();

                // match parameters to props
                foreach (var parameter in element.Parameters)
                {
                    try
                    {
                        if (props.TryGetValue(parameter.Name, out var prop))
                        {
                            matchedParams.Add(parameter.Name);

                            var provider = PropResolver.Resolve(context, element, parameter, prop.Value);

                            element.Props[parameter.Name] = provider ?? throw new YamlComponentException($"Cannot resolve prop '{parameter.Name}' in element {type}.", prop.Key);

                            points += 1;
                        }

                        else if (parameter.HasDefaultValue)
                        {
                            points += 0.5f;
                        }

                        else
                        {
                            throw new YamlComponentException($"Missing required prop '{parameter.Name}' ({parameter.ParameterType}).", node);
                        }
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                // find excessive props
                foreach (var (key, prop) in props)
                {
                    try
                    {
                        if (!matchedParams.Remove(key) && !PropResolver.Resolve(context, element, key, prop.Value))
                            throw new YamlComponentException($"Cannot resolve prop '{key}' in element {type}.", prop.Key);
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                matches.Add(new ElementMatch(element, exceptions, points));
            }

            // sort by relevance descending
            matches.Sort((a, b) => -a.Relevance.CompareTo(b.Relevance));

            // find matching constructor
            foreach (var group in matches.Where(m => m.Exception == null).GroupBy(m => m.Relevance))
            {
                var matched = group.ToArray();

                if (matched.Length == 1)
                    return matched[0].Element;

                throw new YamlComponentException($"Ambiguous element constructor reference: {string.Join(", ", matched.Select(m => m.Element.Constructor))}", node);
            }

            // no matched constructor, so throw for the first failed constructor
            var failed = matches.FirstOrDefault(m => m.Exception != null);

            if (failed != null)
                ExceptionDispatchInfo.Capture(failed.Exception).Throw();

            throw new YamlComponentException($"No public instance constructor in element {type}.", node);
        }
    }
}