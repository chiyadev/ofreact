using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a field or parameter as a ref of an element.
    /// </summary>
    /// <remarks>
    /// Ref fields must be public and the type must be a constructed <see cref="RefObject{T}"/>.
    /// When used on a method parameter, <see cref="RefObject{T}"/> or its unwrapped value will be injected as the argument.
    /// Refs do not cause a rerender even if its value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RefAttribute : Attribute, IElementFieldBinder, IElementMethodParameterProvider
    {
        readonly object _initialValue;

        string _name;

        Func<ofNode, string, object> _get;

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
        /// <param name="container">Either of constructed <see cref="RefObject{T}"/> or <see cref="StateObject{T}"/>.</param>
        /// <param name="initial">Initial value.</param>
        /// <returns>Function to create it.</returns>
        internal static Func<ofNode, string, IContainerObject> GetRefOrStateObjectFactory(Type container, object initial)
        {
            var refCtor = container.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var type    = container.GenericTypeArguments[0];

            if (InternalConstants.IsEmitAvailable)
            {
                var node = Expression.Parameter(typeof(ofNode), "node");
                var key  = Expression.Parameter(typeof(string), "key");

                return Expression.Lambda<Func<ofNode, string, IContainerObject>>(
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

            return (n, k) => (IContainerObject) refCtor.Invoke(new[] { n, k, initial });
        }

        void IElementFieldBinder.Initialize(FieldInfo field)
        {
            if (field.FieldType.GetGenericTypeDefinition() != typeof(RefObject<>))
                throw new ArgumentException($"Field {field} of {field.DeclaringType} must be a type of {typeof(RefObject<>)}");

            _name = field.Name;

            _get = GetRefOrStateObjectFactory(field.FieldType, _initialValue);
        }

        void IElementMethodParameterProvider.Initialize(ParameterInfo parameter)
        {
            _name = parameter.Name;

            if (parameter.ParameterType.GetGenericTypeDefinition() == typeof(RefObject<>))
            {
                _get = GetRefOrStateObjectFactory(parameter.ParameterType, _initialValue);
            }
            else
            {
                //todo: this breaks with aot
                var get = GetRefOrStateObjectFactory(typeof(RefObject<>).MakeGenericType(parameter.ParameterType), _initialValue);

                _get = (n, k) => get(n, k).Current;
            }
        }

        object IElementMethodParameterProvider.GetValue(ofElement element) => _get(element.Node, _name);
        object IElementFieldBinder.GetValue(ofElement element) => _get(element.Node, _name);
    }
}