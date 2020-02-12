using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a field as a ref of an element.
    /// </summary>
    /// <remarks>
    /// Ref fields must be public and the type must be a constructed <see cref="RefObject{T}"/>.
    /// Refs do not cause a rerender even if its value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class RefAttribute : Attribute, IElementFieldBinder
    {
        readonly object _initialValue;

        FieldInfo _field;
        string _name;

        Func<ofNode, string, object> _createRef;

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

        /// <summary>
        /// Used to create a container dynamically.
        /// </summary>
        /// <param name="container">Either <see cref="RefObject{T}"/> or <see cref="StateObject{T}"/> constructed.</param>
        /// <param name="initial">Initial value.</param>
        /// <returns>Function to create it.</returns>
        internal static Func<ofNode, string, object> CreateRefOrStateObject(Type container, object initial)
        {
            var refCtor = container.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var type    = container.GenericTypeArguments[0];

            if (InternalConstants.IsEmitAvailable)
            {
                var node = Expression.Parameter(typeof(ofNode), "node");
                var key  = Expression.Parameter(typeof(string), "key");

                return Expression.Lambda<Func<ofNode, string, object>>(
                                      Expression.New(
                                          refCtor,
                                          node,
                                          key,
                                          initial == null
                                              ? Expression.Default(type) as Expression
                                              : Expression.Convert(Expression.Constant(initial), type)),
                                      node,
                                      key)
                                 .Compile();
            }

            if (initial == null && type.IsValueType)
                initial = Activator.CreateInstance(type);

            return (n, k) => refCtor.Invoke(new[] { n, k, initial });
        }

        public void Initialize(FieldInfo field)
        {
            if (field.FieldType.GetGenericTypeDefinition() != typeof(RefObject<>))
                throw new ArgumentException($"Field {field} of {field.DeclaringType} is not a type of {typeof(RefObject<>)}");

            _field = field;
            _name  = field.Name.ToLowerInvariant();

            _createRef = CreateRefOrStateObject(_field.FieldType, _initialValue);
        }

        public void Bind(ofElement element) => _field.SetValue(element, _createRef(element.Node, _name));
    }
}