using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace ofreact.Yaml
{
    public class KeyPropResolver : IPropResolver
    {
        public IPropProvider Resolve(IYamlComponentBuilder builder, ElementRenderInfo element, ParameterInfo parameter, YamlNode node)
        {
            if (parameter.Name != "key")
                return null;

            if (!(node is YamlScalarNode scalar))
                throw new YamlComponentException("Must be a scalar.", node);

            if (!parameter.ParameterType.IsAssignableFrom(typeof(string)))
                throw new YamlComponentException($"Parameter type {parameter.ParameterType} is not assignable from string.", scalar);

            // declare a variable for this element
            if (!builder.TryDeclareVariable(parameter.Member.DeclaringType, scalar.Value, null, out var variable))
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

            public Expression GetValue(Expression node) => Expression.Constant(_value);
        }
    }
}