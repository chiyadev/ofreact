using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Used internally within ofreact for all reflection-related logic.
    /// </summary>
    public static class InternalReflection
    {
        /// <summary>
        /// Factory to use to generate factory methods.
        /// </summary>
        public static IReflectionFactory Factory { get; set; }

        /// <summary>
        /// Returns a value indicating whether IL emit is available in the calling environment.
        /// </summary>
        public static bool IsEmitAvailable { get; }

        static InternalReflection()
        {
            try
            {
                IsEmitAvailable = Expression.Lambda<Func<bool>>(Expression.Constant(true)).Compile()();
            }
            catch
            {
                IsEmitAvailable = false;
            }

            Factory = IsEmitAvailable
                ? new ExpressionTreeReflectionFactory() as IReflectionFactory
                : new DynamicReflectionFactory();
        }

        public delegate bool PropsEqualityComparerDelegate(object a, object b);

        static readonly ConcurrentDictionary<Type, PropsEqualityComparerDelegate> _propsEqualityComparer = new ConcurrentDictionary<Type, PropsEqualityComparerDelegate>();
        static readonly PropsEqualityComparerDelegate _emptyPropsEqualityComparer = (a, b) => true;

        /// <summary>
        /// Returns true if all fields of element <paramref name="a"/> and <paramref name="b"/> marked with <see cref="PropAttribute"/> are equal.
        /// </summary>
        public static bool PropsEqual(object a, object b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            var type = a.GetType();

            if (type != b.GetType())
                return false;

            if (_propsEqualityComparer.TryGetValue(type, out var comparer))
                return comparer?.Invoke(a, b) ?? true;

            var fields = type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)).ToArray();

            if (fields.Length == 0)
            {
                _propsEqualityComparer[type] = null;
                return true;
            }

            return (_propsEqualityComparer[type] = Factory.GetPropsEqualityComparer(type, fields) ?? _emptyPropsEqualityComparer)?.Invoke(a, b) ?? true;
        }

        public delegate void ElementBinderDelegate(ofElement element);

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

        public delegate IContainerObject ContainerObjectFactoryDelegate(ofNode node, string key, object initial);

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

        public delegate object ElementMethodInvokerDelegate(ofElement element);

        public delegate object[] ElementDependencyListBuilderDelegate(ofElement element);

        /// <summary>
        /// Used internally within ofreact to generate factory methods.
        /// </summary>
        public interface IReflectionFactory
        {
            PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type, FieldInfo[] fields);
            ElementBinderDelegate GetElementBinder(Type type, IElementFieldBinder[] fields, IElementMethodInvoker[] methods);
            ContainerObjectFactoryDelegate GetContainerObjectFactory(Type type);
            ElementMethodInvokerDelegate GetElementMethodInvoker(Type type, MethodInfo method, IElementMethodArgumentProvider[] arguments);
            ElementDependencyListBuilderDelegate GetElementDependencyListBuilder(Type type, FieldInfo[] fields, IElementMethodArgumentProvider[] arguments);
        }

        sealed class DynamicReflectionFactory : IReflectionFactory
        {
            public PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type, FieldInfo[] fields)
            {
                switch (fields.Length)
                {
                    case 0:
                        return null;

                    case 1:
                    {
                        var field = fields[0];

                        return (a, b) =>
                        {
                            var x = field.GetValue(a);
                            var y = field.GetValue(b);

                            return x == y || x != null && x.Equals(y);
                        };
                    }

                    default:
                    {
                        return (a, b) =>
                        {
                            for (var i = 0; i < fields.Length; i++)
                            {
                                var field = fields[i];

                                var x = field.GetValue(a);
                                var y = field.GetValue(b);

                                if (x != y && (x == null || !x.Equals(y)))
                                    return false;
                            }

                            return true;
                        };
                    }
                }
            }

            public ElementBinderDelegate GetElementBinder(Type type, IElementFieldBinder[] fields, IElementMethodInvoker[] methods) => e =>
            {
                foreach (var binder in fields)
                    binder.Field.SetValue(e, binder.GetValue(e));

                foreach (var invoker in methods)
                    invoker.Invoke(e);
            };

            public ContainerObjectFactoryDelegate GetContainerObjectFactory(Type type)
            {
                // type is container
                var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];

                type = type.GenericTypeArguments[0];

                // create default value for type if it's struct
                var defaultValue = null as object;

                if (type.IsValueType)
                    defaultValue = Activator.CreateInstance(type);

                return (n, k, i) => (IContainerObject) ctor.Invoke(new[] { n, k, i ?? defaultValue });
            }

            public ElementMethodInvokerDelegate GetElementMethodInvoker(Type type, MethodInfo method, IElementMethodArgumentProvider[] arguments) => e =>
            {
                var args = new object[arguments.Length];

                for (var i = 0; i < arguments.Length; i++)
                    args[i] = arguments[i].GetValue(e);

                return method.Invoke(e, args);
            };

            public ElementDependencyListBuilderDelegate GetElementDependencyListBuilder(Type type, FieldInfo[] fields, IElementMethodArgumentProvider[] arguments) => e =>
            {
                var deps = new object[fields.Length + arguments.Length];

                for (var i = 0; i < fields.Length; i++)
                    deps[i] = fields[i].GetValue(e);

                for (var i = 0; i < arguments.Length; i++)
                    deps[fields.Length + i] = arguments[i].GetValue(e);

                return deps;
            };
        }

        sealed class ExpressionTreeReflectionFactory : IReflectionFactory
        {
            public PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type, FieldInfo[] fields)
            {
                var x = Expression.Parameter(type, "x");
                var y = Expression.Parameter(type, "y");

                var body = null as Expression;

                foreach (var field in fields)
                {
                    var other = Expression.Equal(
                        Expression.Field(x, field),
                        Expression.Field(y, field));

                    body = body == null
                        ? other
                        : Expression.AndAlso(body, other);
                }

                if (body == null)
                    return null;

                var a = Expression.Parameter(typeof(object), "a");
                var b = Expression.Parameter(typeof(object), "b");

                return Expression.Lambda<PropsEqualityComparerDelegate>(
                                      Expression.Invoke(
                                          Expression.Lambda(body, x, y),
                                          Expression.Convert(a, type),
                                          Expression.Convert(b, type)),
                                      a, b)
                                 .Compile();
            }

            static readonly MethodInfo _fieldSetValueMethod = typeof(FieldInfo).GetMethod(nameof(FieldInfo.SetValue), new[] { typeof(object), typeof(object) });

            static Expression BuildFieldSetter(Expression instance, FieldInfo field, Expression value)
            {
                // if field is readonly, we must use reflection to set it
                if (field.IsInitOnly)
                    return Expression.Call(Expression.Constant(field), _fieldSetValueMethod, instance, value);

                return Expression.Assign(Expression.Field(instance, field), value);
            }

            static readonly MethodInfo _fieldBinderGetValueMethod = typeof(IElementFieldBinder).GetMethod(nameof(IElementFieldBinder.GetValue));
            static readonly MethodInfo _methodInvokerInvokeMethod = typeof(IElementMethodInvoker).GetMethod(nameof(IElementMethodInvoker.Invoke));

            public ElementBinderDelegate GetElementBinder(Type type, IElementFieldBinder[] fields, IElementMethodInvoker[] methods)
            {
                var element = Expression.Parameter(type, "element");

                var body = new List<Expression>(fields.Length + methods.Length);

                // set fields
                foreach (var binder in fields)
                    body.Add(BuildFieldSetter(element, binder.Field, Expression.Call(Expression.Constant(binder), _fieldBinderGetValueMethod, element)));

                // invoke methods
                foreach (var invoker in methods)
                    body.Add(Expression.Call(Expression.Constant(invoker), _methodInvokerInvokeMethod, element));

                // wrap main body in lambda that takes ofElement and downcasts it
                var e = Expression.Parameter(typeof(ofElement), "element");

                return Expression.Lambda<ElementBinderDelegate>(
                                      Expression.Invoke(
                                          Expression.Lambda(
                                              Expression.Block(body),
                                              element),
                                          Expression.Convert(e, type)),
                                      e)
                                 .Compile();
            }

            public ContainerObjectFactoryDelegate GetContainerObjectFactory(Type type)
            {
                // type is container
                var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];

                type = type.GenericTypeArguments[0];

                var node    = Expression.Parameter(typeof(ofNode), "node");
                var key     = Expression.Parameter(typeof(string), "key");
                var initial = Expression.Parameter(typeof(object), "initial");

                return Expression.Lambda<ContainerObjectFactoryDelegate>(
                                      Expression.New(
                                          ctor,
                                          node,
                                          key,
                                          Expression.Condition(
                                              Expression.ReferenceEqual(initial, Expression.Constant(null, typeof(object))),
                                              Expression.Default(type),
                                              Expression.Convert(initial, type))),
                                      node,
                                      key,
                                      initial)
                                 .Compile();
            }

            static readonly MethodInfo _methodArgumentProviderGetValueMethod = typeof(IElementMethodArgumentProvider).GetMethod(nameof(IElementMethodArgumentProvider.GetValue));

            public ElementMethodInvokerDelegate GetElementMethodInvoker(Type type, MethodInfo method, IElementMethodArgumentProvider[] arguments)
            {
                var element = Expression.Parameter(typeof(ofElement), "element");

                Expression body = Expression.Call(
                    Expression.Convert(element, type),
                    method,
                    arguments.Select(p => Expression.Convert(Expression.Call(Expression.Constant(p), _methodArgumentProviderGetValueMethod, element), p.Parameter.ParameterType)));

                if (method.ReturnType == typeof(void))
                    body = Expression.Block(body, Expression.Constant(null, typeof(object)));

                return Expression.Lambda<ElementMethodInvokerDelegate>(body, element).Compile();
            }

            public ElementDependencyListBuilderDelegate GetElementDependencyListBuilder(Type type, FieldInfo[] fields, IElementMethodArgumentProvider[] arguments)
            {
                if (fields.Length == 0 && arguments.Length == 0)
                    return x => Array.Empty<object>();

                var element = Expression.Parameter(type, "element");

                var deps = new Expression[fields.Length + arguments.Length];

                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];

                    Expression value = Expression.Field(element, field);

                    // if struct, box
                    if (field.FieldType.IsValueType)
                        value = Expression.Convert(value, typeof(object));

                    deps[i] = value;
                }

                for (var i = 0; i < arguments.Length; i++)
                    deps[fields.Length + i] = Expression.Call(Expression.Constant(arguments[i]), _methodArgumentProviderGetValueMethod, element);

                // wrap main body in lambda that takes ofElement and downcasts it
                var e = Expression.Parameter(typeof(ofElement), "element");

                return Expression.Lambda<ElementDependencyListBuilderDelegate>(
                                      Expression.Invoke(
                                          Expression.Lambda(
                                              Expression.NewArrayInit(typeof(object), deps),
                                              element),
                                          Expression.Convert(e, type)),
                                      e)
                                 .Compile();
            }
        }
    }
}