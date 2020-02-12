using System;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a field or parameter as a state of an element.
    /// </summary>
    /// <remarks>
    /// State fields must be a constructed <see cref="StateObject{T}"/>.
    /// When used on a method parameter, <see cref="StateObject{T}"/> or its unwrapped value will be injected as the argument.
    /// States will cause a rerender when its value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class StateAttribute : Attribute, IElementFieldBinder, IElementMethodParameterProvider
    {
        readonly object _initialValue;

        string _name;
        Func<ofNode, string, object> _get;

        /// <summary>
        /// Creates a new <see cref="StateAttribute"/>.
        /// </summary>
        public StateAttribute() { }

        /// <summary>
        /// Creates a new <see cref="StateAttribute"/> with the given initial value.
        /// </summary>
        /// <param name="initialValue">Initial value of the stateful value.</param>
        public StateAttribute(object initialValue)
        {
            _initialValue = initialValue;
        }

        void IElementFieldBinder.Initialize(FieldInfo field)
        {
            if (field.FieldType.GetGenericTypeDefinition() != typeof(StateObject<>))
                throw new ArgumentException($"Field {field} of {field.DeclaringType} is not a type of {typeof(StateObject<>)}");

            _name = field.Name;

            _get = RefAttribute.GetRefOrStateObjectFactory(field.FieldType, _initialValue);
        }

        void IElementMethodParameterProvider.Initialize(ParameterInfo parameter)
        {
            _name = parameter.Name;

            if (parameter.ParameterType.GetGenericTypeDefinition() == typeof(StateObject<>))
            {
                _get = RefAttribute.GetRefOrStateObjectFactory(parameter.ParameterType, _initialValue);
            }
            else
            {
                //todo: this breaks with aot
                var get = RefAttribute.GetRefOrStateObjectFactory(typeof(StateObject<>).MakeGenericType(parameter.ParameterType), _initialValue);

                _get = (n, k) => get(n, k).Current;
            }
        }

        object IElementFieldBinder.GetValue(ofElement element) => _get(element.Node, _name);
        object IElementMethodParameterProvider.GetValue(ofElement element) => _get(element.Node, _name);
    }
}