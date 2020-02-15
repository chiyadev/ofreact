using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ofreact
{
    public delegate bool PropsEqualityComparerDelegate(ofElement a, ofElement b);

    public delegate void ElementBinderDelegate(ofElement element);

    public delegate IContainerObject ContainerObjectFactoryDelegate(ofNode node, string key, object initial);

    public delegate object ElementMethodInvokerDelegate(ofElement element);

    public delegate object[] ElementDependencyListBuilderDelegate(ofElement element);

    /// <summary>
    /// Used internally within ofreact for all reflection-related logic.
    /// </summary>
    public static class InternalReflection
    {
        /// <summary>
        /// Factory to use to generate factory methods.
        /// </summary>
        /// <remarks>
        /// <see cref="ExpressionTreeReflectionFactory"/> is used by default.
        /// In environments without IL emit support, <see cref="DynamicReflectionFactory"/> is used instead.
        /// </remarks>
        public static IReflectionFactory Factory { get; set; }

        static InternalReflection()
        {
            Factory = InternalConstants.IsEmitAvailable
                ? ExpressionTreeReflectionFactory.Instance
                : DynamicReflectionFactory.Instance;
        }

        static readonly ConcurrentDictionary<Type, PropsEqualityComparerDelegate> _propsEqualityComparer = new ConcurrentDictionary<Type, PropsEqualityComparerDelegate>();
        static readonly PropsEqualityComparerDelegate _emptyPropsEqualityComparer = (a, b) => true;

        /// <summary>
        /// Returns true if all props of element <paramref name="a"/> and <paramref name="b"/> are equal.
        /// </summary>
        public static bool PropsEqual(ofElement a, ofElement b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            var type = a.GetType();

            if (type != b.GetType())
                return false;

            if (_propsEqualityComparer.TryGetValue(type, out var comparer))
                return comparer(a, b);

            return (_propsEqualityComparer[type] = Factory.GetPropsEqualityComparer(type) ?? _emptyPropsEqualityComparer)(a, b);
        }

        static readonly ConcurrentDictionary<Type, ElementBinderDelegate> _elementBinder = new ConcurrentDictionary<Type, ElementBinderDelegate>();

        /// <summary>
        /// Binds attribute-bound members of the given element to the element's <see cref="ofElement.Node"/>.
        /// </summary>
        public static void BindElement(ofElement element)
        {
            var type = element.GetType();

            if (_elementBinder.TryGetValue(type, out var binder))
            {
                binder?.Invoke(element);
                return;
            }

            var fields  = new List<IElementFieldBinder>();
            var methods = new List<IElementMethodInvoker>();

            foreach (var field in type.GetAllFields())
            foreach (var fieldBinder in field.GetCustomAttributes().OfType<IElementFieldBinder>())
            {
                fieldBinder.Initialize(field);

                fields.Add(fieldBinder);
            }

            foreach (var method in type.GetAllMethods())
            foreach (var methodInvoker in method.GetCustomAttributes().OfType<IElementMethodInvoker>())
            {
                var parameters = method.GetParameters();
                var arguments  = new IElementMethodArgumentProvider[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var provider  = parameter.GetCustomAttributes().OfType<IElementMethodArgumentProvider>().FirstOrDefault();

                    if (provider == null && !methodInvoker.AllowUnknownParameters)
                        throw new ArgumentException($"Cannot find providers for parameter {parameter} of {method} of {type}.");

                    provider?.Initialize(parameter);

                    arguments[i] = provider;
                }

                methodInvoker.Initialize(method, arguments);

                methods.Add(methodInvoker);
            }

            if (fields.Count == 0 && methods.Count == 0)
            {
                _elementBinder[type] = null;
                return;
            }

            (_elementBinder[type] = Factory.GetElementBinder(type, fields.ToArray(), methods.ToArray()))(element);
        }

        static readonly ConcurrentDictionary<Type, ContainerObjectFactoryDelegate> _containerObjectFactory = new ConcurrentDictionary<Type, ContainerObjectFactoryDelegate>();

        /// <summary>
        /// Returns a factory method for creating a <see cref="RefObject{T}"/>.
        /// </summary>
        /// <param name="type">Type of the referenced value or ref object.</param>
        /// <param name="wrapped">True if the given type was wrapped in <see cref="RefObject{T}"/> as bound generic.</param>
        public static ContainerObjectFactoryDelegate GetRefObjectFactory(Type type, out bool wrapped)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(RefObject<>))
            {
                //todo: this breaks with aot
                type    = typeof(RefObject<>).MakeGenericType(type);
                wrapped = true;
            }
            else
            {
                wrapped = false;
            }

            if (_containerObjectFactory.TryGetValue(type, out var factory))
                return factory;

            return _containerObjectFactory[type] = Factory.GetContainerObjectFactory(type);
        }

        /// <summary>
        /// Returns a factory method for creating a <see cref="StateObject{T}"/>.
        /// </summary>
        /// <param name="type">Type of the state value or state object.</param>
        /// <param name="wrapped">True if the given type was wrapped in <see cref="StateObject{T}"/> as bound generic.</param>
        public static ContainerObjectFactoryDelegate GetStateObjectFactory(Type type, out bool wrapped)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(StateObject<>))
            {
                //todo: this breaks with aot
                type    = typeof(StateObject<>).MakeGenericType(type);
                wrapped = true;
            }
            else
            {
                wrapped = false;
            }

            if (_containerObjectFactory.TryGetValue(type, out var factory))
                return factory;

            return _containerObjectFactory[type] = Factory.GetContainerObjectFactory(type);
        }

        internal static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            do
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return field;
            }
            while ((type = type.BaseType) != null);
        }

        internal static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            do
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    yield return method;
            }
            while ((type = type.BaseType) != null);
        }

        internal static FieldInfo GetAllField(this Type type, string name)
        {
            do
            {
                var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (field != null)
                    return field;
            }
            while ((type = type.BaseType) != null);

            return null;
        }

        internal static MethodInfo GetAllMethod(this Type type, string name)
        {
            do
            {
                var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (method != null)
                    return method;
            }
            while ((type = type.BaseType) != null);

            return null;
        }
    }

    /// <summary>
    /// Used internally within ofreact to generate factory methods.
    /// </summary>
    public interface IReflectionFactory
    {
        PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type);
        ElementBinderDelegate GetElementBinder(Type type, IElementFieldBinder[] fields, IElementMethodInvoker[] methods);
        ContainerObjectFactoryDelegate GetContainerObjectFactory(Type type);
        ElementMethodInvokerDelegate GetElementMethodInvoker(Type type, MethodInfo method, IElementMethodArgumentProvider[] arguments);
        ElementDependencyListBuilderDelegate GetElementDependencyListBuilder(Type type, FieldInfo[] fields, IElementMethodArgumentProvider[] arguments);
    }
}