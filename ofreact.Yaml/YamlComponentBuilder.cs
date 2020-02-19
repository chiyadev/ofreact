using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    /// <summary>
    /// Used to handle one key-value mapping in the document root.
    /// </summary>
    public interface IComponentPartHandler
    {
        bool Handle(IYamlComponentBuilder builder, string name, YamlNode node);
    }

    /// <summary>
    /// Used to find an element type from a string.
    /// </summary>
    public interface IElementTypeResolver
    {
        Type Resolve(IYamlComponentBuilder builder, string name);
    }

    /// <summary>
    /// Used to create a prop provider from a YAML node.
    /// </summary>
    public interface IPropResolver
    {
        IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node);
    }

    public interface IYamlComponentBuilder : IComponentBuilder
    {
        IComponentPartHandler PartHandler { get; set; }
        IElementTypeResolver TypeResolver { get; set; }
        IPropResolver PropResolver { get; set; }

        ElementRenderInfo BuildElement(YamlNode node);
    }

    /// <summary>
    /// Transforms a <see cref="YamlDocument"/> to an <see cref="ofElement"/>.
    /// </summary>
    public class YamlComponentBuilder : ComponentBuilderBase, IYamlComponentBuilder
    {
        readonly YamlMappingNode _mapping;

        public IComponentPartHandler PartHandler { get; set; } =
            new CompositePartHandler(
                new NamePartHandler());

        public IElementTypeResolver TypeResolver { get; set; } =
            new PrefixedElementResolver("of",
                new CompositeElementResolver(
                    new AssemblyElementResolver(typeof(ofElement).Assembly),
                    new AssemblyElementResolver(typeof(YamlComponentBuilder).Assembly)));

        public IPropResolver PropResolver { get; set; } =
            new CompositePropResolver(
                new AttributePropResolver(),
                new KeyPropResolver(),
                new PrimitivePropResolver());

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

        protected override ElementRenderInfo Render(Expression node)
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

                if (!PartHandler.Handle(this, keyScalar.Value, value))
                    throw new YamlComponentException($"Invalid component part '{keyScalar.Value}'.", key);
            }

            if (render == null)
                return null;

            return BuildElement(render);
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

        public ElementRenderInfo BuildElement(YamlNode node)
        {
            switch (node)
            {
                // element
                case YamlMappingNode mapping:
                    if (mapping.Children.Count > 1)
                        throw new YamlComponentException("Mapping must have one key that indicates the element type.", mapping);

                    var (key, value) = mapping.Children.First();

                    if (!(key is YamlScalarNode typeScalar))
                        throw new YamlComponentException("Must be a scalar.", key);

                    if (!(value is YamlMappingNode propsMapping))
                        throw new YamlComponentException("Must be a mapping.", value);

                    // resolve type
                    var type = TypeResolver.Resolve(this, typeScalar.Value) ?? throw new YamlComponentException($"Cannot resolve element '{typeScalar.Value}'.", typeScalar);

                    // build prop dictionary
                    var props = propsMapping.ToDictionary(x => (x.Key as YamlScalarNode ?? throw new YamlComponentException("Must be a scalar.", x.Key)).Value, x => new YamlProp(x.Key, x.Value));

                    return BuildElementWithProps(type, typeScalar, props);

                // fragment
                case YamlSequenceNode sequence:
                    if (sequence.Children.Count == 1)
                        return BuildElement(sequence[0]);

                    return new FragmentRenderInfo(sequence.Select(BuildElement));

                default:
                    throw new YamlComponentException("Must be a mapping or sequence.", node);
            }
        }

        sealed class ElementMatch
        {
            public readonly ElementRenderInfo Element;
            public readonly Exception Exception;
            public readonly int Relevance;

            public ElementMatch(ElementRenderInfo element, Exception exception, int relevance)
            {
                Element   = element;
                Exception = exception;
                Relevance = relevance;
            }
        }

        ElementRenderInfo BuildElementWithProps(Type type, YamlNode node, IReadOnlyDictionary<string, YamlProp> props)
        {
            var success   = new List<ElementMatch>();
            var exception = new List<ElementMatch>();

            foreach (var constructor in type.GetConstructors())
            {
                var element   = new ElementRenderInfo(type, constructor);
                var relevance = 0;

                try
                {
                    var matchedProps = new HashSet<string>();

                    // match parameters to props
                    foreach (var parameter in element.Parameters)
                    {
                        if (props.TryGetValue(parameter.Name, out var prop))
                        {
                            matchedProps.Add(parameter.Name);

                            var provider = PropResolver.Resolve(this, element, parameter, prop.Value);

                            element.Props[parameter.Name] = provider ?? throw new YamlComponentException($"Cannot resolve prop '{parameter.Name}' in element {type}.", prop.Value);
                        }

                        else if (!parameter.HasDefaultValue)
                        {
                            throw new YamlComponentException($"Missing required prop '{parameter.Name}' ({parameter.ParameterType}).", node);
                        }
                    }

                    relevance = matchedProps.Count;

                    // find excessive props
                    foreach (var (key, prop) in props)
                    {
                        if (!matchedProps.Remove(key))
                            throw new YamlComponentException($"Cannot resolve prop '{key}' in element {type}.", prop.Key);
                    }

                    success.Add(new ElementMatch(element, null, relevance));
                }
                catch (Exception e)
                {
                    exception.Add(new ElementMatch(element, e, relevance));
                }
            }

            if (success.Count != 0)
                foreach (var group in success.OrderByDescending(m => m.Relevance).GroupBy(m => m.Relevance))
                {
                    var matches = group.ToArray();

                    if (matches.Length == 1)
                        return matches[0].Element;

                    throw new YamlComponentException($"Ambiguous element constructor reference: {string.Join(", ", matches.Select(m => m.Element.Constructor))}", node);
                }

            if (exception.Count != 0)
                ExceptionDispatchInfo.Capture(exception.OrderByDescending(m => m.Relevance).First().Exception).Throw();

            throw new YamlComponentException($"No public instance constructor in element {type}.", node);
        }
    }
}