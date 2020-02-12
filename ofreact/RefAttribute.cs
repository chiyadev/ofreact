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

        public void Initialize(FieldInfo field)
        {
            if (field.FieldType.GetGenericTypeDefinition() != typeof(RefObject<>))
                throw new ArgumentException($"Field {field} of {field.DeclaringType} is not a type of {typeof(RefObject<>)}");

            _field = field;
            _name  = field.Name.ToLowerInvariant();

            var refCtor = field.FieldType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var type    = field.FieldType.GenericTypeArguments[0];

            if (InternalConstants.IsEmitAvailable)
            {
                var node = Expression.Parameter(typeof(ofNode), "node");
                var key  = Expression.Parameter(typeof(string), "key");

                _createRef = Expression.Lambda<Func<ofNode, string, object>>(
                                            Expression.New(
                                                refCtor,
                                                node,
                                                key,
                                                _initialValue == null
                                                    ? Expression.Default(type) as Expression
                                                    : Expression.Convert(Expression.Constant(_initialValue), type)),
                                            node,
                                            key)
                                       .Compile();
            }
            else
            {
                var initial = _initialValue;

                if (initial == null && type.IsValueType)
                    initial = Activator.CreateInstance(type);

                _createRef = (n, k) => refCtor.Invoke(new[] { n, k, initial });
            }
        }

        public void Bind(ofElement element) => _field.SetValue(element, _createRef(element.Node, _name));
    }
}