using System;
using System.Linq.Expressions;
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
    public class StateAttribute : Attribute, IBinderFieldProvider, IBinderMethodParameterProvider
    {
        readonly object _initialValue;

        /// <summary>
        /// Creates a new <see cref="RefAttribute"/>.
        /// </summary>
        public StateAttribute() { }

        /// <summary>
        /// Creates a new <see cref="RefAttribute"/> with the given initial value.
        /// </summary>
        /// <param name="initialValue">Initial value of the referenced value.</param>
        public StateAttribute(object initialValue)
        {
            _initialValue = initialValue;
        }

        public Expression GetValue(Expression element, FieldInfo field)
        {
            var type = field.FieldType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(StateObject<>))
                return ElementBinder.BuildContainerInstantiation(type, element, Expression.Constant(field.Name), _initialValue);

            throw new ArgumentException($"Field {field} of {field.DeclaringType} must be a type of {typeof(StateObject<>)}");
        }

        public Expression GetValue(Expression element, ParameterInfo parameter)
        {
            var type = parameter.ParameterType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(StateObject<>))
                return ElementBinder.BuildContainerInstantiation(type, element, Expression.Constant(parameter.Name), _initialValue);

            // todo: this breaks with aot
            return ElementBinder.BuildContainerValueAccess(ElementBinder.BuildContainerInstantiation(typeof(StateObject<>).MakeGenericType(type), element, Expression.Constant(parameter.Name), _initialValue));
        }
    }
}