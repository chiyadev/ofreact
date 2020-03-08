using System.Linq.Expressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element prop that represents the key of the element.
    /// </summary>
    public class KeyPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementBuilder element, YamlNode node)
        {
            if (prop.Type != typeof(ElementKey))
                return null;

            return ResolveInternal(context, element, node);
        }

        public bool Resolve(ComponentBuilderContext context, string prop, ElementBuilder element, YamlNode node)
        {
            if (prop != "key")
                return false;

            return ResolveInternal(context, element, node) != null;
        }

        static Provider ResolveInternal(ComponentBuilderContext context, ElementBuilder element, YamlNode node)
        {
            var value = node.ToScalar().Value;

            // declare a variable for this element
            if (!context.TryDeclareVariable(element.Type, value, null, out var variable))
                throw new YamlComponentException($"Variable '{value}' is already declared.", node);

            // assign to this variable when element is constructed
            if (element.Assignee != null)
                throw new YamlComponentException("Cannot specify multiple keys for an element.", node);

            element.Assignee = variable;

            return new Provider(value);
        }

        sealed class Provider : IPropProvider
        {
            readonly string _value;

            public Provider(string value)
            {
                _value = value;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Convert(Expression.Constant(_value), typeof(ElementKey));
        }
    }
}