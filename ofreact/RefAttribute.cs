using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a field or parameter as a ref of an element.
    /// </summary>
    /// <remarks>
    /// Ref fields must be a constructed <see cref="RefObject{T}"/>.
    /// When used on a method parameter, <see cref="RefObject{T}"/> or its unwrapped value will be injected as the argument.
    /// Refs do not cause a rerender even if its value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RefAttribute : Attribute, IBinderFieldProvider, IBinderMethodParameterProvider
    {
        readonly object _initialValue;

        /// <summary>
        /// Creates a new <see cref="RefAttribute"/>.
        /// </summary>
        public RefAttribute() { }

        /// <summary>
        /// Creates a new <see cref="RefAttribute"/> with the given initial value.
        /// </summary>
        /// <param name="initialValue">Initial value of the referenced value.</param>
        public RefAttribute(object initialValue)
        {
            _initialValue = initialValue;
        }

        public Expression GetValue(Expression element, FieldInfo field)
        {
            var type = field.FieldType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RefObject<>))
                return ElementBinder.BuildContainerInstantiation(type, element, Expression.Constant(field.Name), _initialValue);

            throw new ArgumentException($"Field {field} of {field.DeclaringType} must be a type of {typeof(RefObject<>)}");
        }

        public Expression GetValue(Expression element, ParameterInfo parameter)
        {
            var type = parameter.ParameterType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RefObject<>))
                return ElementBinder.BuildContainerInstantiation(type, element, Expression.Constant(parameter.Name), _initialValue);

            // todo: this breaks with aot
            return ElementBinder.BuildContainerValueAccess(ElementBinder.BuildContainerInstantiation(typeof(RefObject<>).MakeGenericType(type), element, Expression.Constant(parameter.Name), _initialValue));
        }
    }
}