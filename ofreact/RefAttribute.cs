using System;
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
    public class RefAttribute : Attribute, IElementFieldBinder, IElementMethodArgumentProvider
    {
        readonly object _initialValue;

        string _name;
        ContainerObjectFactoryDelegate _create;
        bool _wrapped;

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

        public FieldInfo Field { get; private set; }
        public ParameterInfo Parameter { get; private set; }

        void IElementFieldBinder.Initialize(FieldInfo field)
        {
            Field   = field;
            _name   = field.Name;
            _create = InternalReflection.GetRefObjectFactory(field.FieldType, out _wrapped);

            if (_wrapped)
                throw new ArgumentException($"Field {field} of {field.DeclaringType} must be a type of {typeof(RefObject<>)}");
        }

        void IElementMethodArgumentProvider.Initialize(ParameterInfo parameter)
        {
            Parameter = parameter;
            _name     = parameter.Name;
            _create   = InternalReflection.GetRefObjectFactory(parameter.ParameterType, out _wrapped);
        }

        object IElementFieldBinder.GetValue(ofElement element)
        {
            var container = _create(element.Node, _name, _initialValue);

            if (_wrapped)
                return container.Current;

            return container;
        }

        object IElementMethodArgumentProvider.GetValue(ofElement element)
        {
            var container = _create(element.Node, _name, _initialValue);

            if (_wrapped)
                return container.Current;

            return container;
        }
    }
}