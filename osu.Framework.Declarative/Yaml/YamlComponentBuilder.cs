using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
        /// <param name="context">Builder context.</param>
        /// <param name="name">Name of the prop.</param>
        /// <param name="element">Element containing this prop that is currently being built.</param>
        /// <param name="parameter">Parameter information of the prop. This may be null.</param>
        /// <param name="node">YAML node of the prop.</param>
        IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node);
    }

    public interface IYamlComponentBuilder : IComponentBuilder
    {
        IComponentPartHandler PartHandler { get; set; }
        IElementTypeResolver ElementResolver { get; set; }
        IPropResolver PropResolver { get; set; }

        ElementRenderInfo BuildElement(ComponentBuilderContext context, YamlNode node);
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
                new KeyPropResolver(),
                new ChildrenPropResolver(),
                new PrimitivePropResolver());

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

        static YamlComponentException WrapException(YamlNode node, Exception e)
        {
            if (e is YamlComponentException yamlException)
                return yamlException;

            return new YamlComponentException(node, e);
        }

        protected override ElementRenderInfo Render(ComponentBuilderContext context)
        {
            var render = null as YamlNode;

            foreach (var (key, value) in _mapping)
            {
                if (!(key is YamlScalarNode keyScalar))
                    throw new YamlComponentException("Must be a scalar.", key);

                if (keyScalar.Value == "render")
                {
                    render = value;
                    continue;
                }

                try
                {
                    if (!PartHandler.Handle(context, keyScalar.Value, value))
                        throw new YamlComponentException($"Invalid component part '{keyScalar.Value}'.", key);
                }
                catch (Exception e)
                {
                    context.OnException(WrapException(value, e));
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

        public ElementRenderInfo BuildElement(ComponentBuilderContext context, YamlNode node)
        {
            try
            {
                switch (node)
                {
                    // element
                    case YamlMappingNode mapping:
                        if (mapping.Children.Count == 0)
                            return new EmptyRenderInfo();

                        if (mapping.Children.Count > 1)
                            throw new YamlComponentException("Mapping must have one key that indicates the element type.", mapping);

                        var (key, value) = mapping.Children.First();

                        if (!(key is YamlScalarNode typeScalar))
                            throw new YamlComponentException("Must be a scalar.", key);

                        // resolve type
                        var type = ElementResolver.Resolve(context, typeScalar.Value) ?? throw new YamlComponentException($"Cannot resolve element '{typeScalar.Value}'.", typeScalar);

                        // build prop dictionary
                        Dictionary<string, YamlProp> props;

                        switch (value)
                        {
                            case YamlMappingNode propsMapping:
                                props = propsMapping.ToDictionary(x => (x.Key as YamlScalarNode ?? throw new YamlComponentException("Must be a scalar.", x.Key)).Value, x => new YamlProp(x.Key, x.Value));
                                break;

                            case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                                props = new Dictionary<string, YamlProp>();
                                break;

                            default:
                                throw new YamlComponentException("Must be a mapping.", value);
                        }

                        return BuildElementWithProps(context, type, typeScalar, props);

                    // fragment
                    case YamlSequenceNode sequence:
                        var elements = sequence.Select(n => BuildElement(context, n)).Where(e => !(e is EmptyRenderInfo)).ToArray();

                        return elements.Length switch
                        {
                            0 => new EmptyRenderInfo(),
                            1 => elements[0],
                            _ => new FragmentRenderInfo(elements)
                        };

                    case YamlScalarNode scalar when string.IsNullOrEmpty(scalar.Value):
                        return new EmptyRenderInfo();

                    default:
                        throw new YamlComponentException("Must be a mapping or sequence.", node);
                }
            }
            catch (Exception e)
            {
                context.OnException(e);

                return new EmptyRenderInfo();
            }
        }

        sealed class ElementMatch
        {
            public readonly ElementRenderInfo Element;
            public readonly Exception Exception;
            public readonly float Relevance;

            public ElementMatch(ElementRenderInfo element, Exception exception, float points)
            {
                Element   = element;
                Exception = exception;

                // relevance is relative
                Relevance = points / element.Parameters.Length;
            }
        }

        ElementRenderInfo BuildElementWithProps(ComponentBuilderContext context, Type type, YamlNode node, IReadOnlyDictionary<string, YamlProp> props)
        {
            var success = new List<ElementMatch>();
            var fail    = new List<ElementMatch>();

            foreach (var constructor in type.GetConstructors())
            {
                var element    = new ElementRenderInfo(type, constructor);
                var exceptions = new List<Exception>();
                var points     = 0f;

                var matchedParams = new HashSet<string>();

                // match parameters to props
                foreach (var parameter in element.Parameters)
                {
                    try
                    {
                        if (props.TryGetValue(parameter.Name, out var prop))
                        {
                            matchedParams.Add(parameter.Name);

                            var provider = PropResolver.Resolve(context, parameter.Name, element, parameter, prop.Value);

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
                        if (!matchedParams.Remove(key))
                        {
                            var provider = PropResolver.Resolve(context, key, element, null, prop.Value);

                            element.Props[key] = provider ?? throw new YamlComponentException($"Cannot resolve prop '{key}' in element {type}.", prop.Key);

                            points += 1;
                        }
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                if (exceptions.Count == 0)
                    success.Add(new ElementMatch(element, null, points));
                else
                    fail.Add(new ElementMatch(element, new AggregateException(exceptions), points));
            }

            if (success.Count != 0)
                foreach (var group in success.OrderByDescending(m => m.Relevance).GroupBy(m => m.Relevance))
                {
                    var matches = group.ToArray();

                    if (matches.Length == 1)
                        return matches[0].Element;

                    throw new YamlComponentException($"Ambiguous element constructor reference: {string.Join(", ", matches.Select(m => m.Element.Constructor))}", node);
                }

            if (fail.Count != 0)
                ExceptionDispatchInfo.Capture(fail.OrderByDescending(m => m.Relevance).First().Exception).Throw();

            throw new YamlComponentException($"No public instance constructor in element {type}.", node);
        }
    }
}