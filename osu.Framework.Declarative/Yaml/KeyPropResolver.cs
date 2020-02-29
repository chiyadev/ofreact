using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    public class KeyPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, string name, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            if (name != "key")
                return null;

            if (!(node is YamlScalarNode scalar))
                throw new YamlComponentException("Must be a scalar.", node);

            if (parameter?.ParameterType.IsAssignableFrom(typeof(string)) == false)
                throw new YamlComponentException($"Parameter type {parameter.ParameterType} is not assignable from string.", scalar);

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