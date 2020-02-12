using System;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a field as a state of an element.
    /// </summary>
    /// <remarks>
    /// State fields must be public and the type must be a constructed <see cref="StateObject{T}"/>.
    /// States will cause a rerender when its value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class StateAttribute : Attribute, IElementFieldBinder
    {
        readonly object _initialValue;

        FieldInfo _field;
        string _name;

        Func<ofNode, string, object> _createState;

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

        public void Initialize(FieldInfo field)
        {
            if (field.FieldType.GetGenericTypeDefinition() != typeof(StateObject<>))
                throw new ArgumentException($"Field {field} of {field.DeclaringType} is not a type of {typeof(StateObject<>)}");

            _field = field;
            _name  = field.Name.ToLowerInvariant();

            _createState = RefAttribute.CreateRefOrStateObject(_field.FieldType, _initialValue);
        }

        public void Bind(ofElement element) => _field.SetValue(element, _createState(element.Node, _name));
    }
}