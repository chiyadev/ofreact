using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Implements <see cref="IReflectionFactory"/> using expression trees.
    /// </summary>
    public class ExpressionTreeReflectionFactory : IReflectionFactory
    {
        public static readonly IReflectionFactory Instance = new ExpressionTreeReflectionFactory();

        ExpressionTreeReflectionFactory() { }

        public PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type)
        {
            var x = Expression.Parameter(type, "x");
            var y = Expression.Parameter(type, "y");

            var body = null as Expression;

            foreach (var field in type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)))
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

            var a = Expression.Parameter(typeof(ofElement), "a");
            var b = Expression.Parameter(typeof(ofElement), "b");

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