using System.Linq.Expressions;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    public class KeyPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementBuilder element, YamlNode node)
        {
            if (prop.Type != typeof(ElementKey))
                return null;

            if (!(node is YamlScalarNode scalar))
                throw new YamlComponentException("Must be a scalar.", node);

            // declare a variable for this element
            if (!context.TryDeclareVariable(element.Type, scalar.Value, null, out var variable))
                throw new YamlComponentException($"Variable '{scalar.Value}' is already declared.", scalar);

            // assign to this variable when element is constructed
            element.Assignee = variable;

            return new Provider(scalar.Value);
        }

        sealed class Provider : IPropProvider
        {
            readonly string _value;

            public Provider(string value)
            {
                _value = value;
            }

            public Expression GetValue(ComponentBuilderContext context) => Expression.Constant(_value);
        }
    }
}