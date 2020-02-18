using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    public interface IBinderFieldProvider
    {
        Expression GetValue(Expression element, FieldInfo field);
    }

    public interface IBinderMethodInvoker
    {
        Expression Build(Expression element, MethodInfo method);
    }

    public interface IBinderMethodParameterProvider
    {
        Expression GetValue(Expression element, ParameterInfo parameter);
    }

    /// <summary>
    /// Responsible for binding attribute-bound element instance members.
    /// </summary>
    public static class ElementBinder
    {
        static readonly ConcurrentDictionary<Type, Action<ofElement>> _binder = new ConcurrentDictionary<Type, Action<ofElement>>();

        public static void BindAttributes(ofElement element)
        {
            if (element == null)
                return;

            var type = element.GetType();

            if (_binder.TryGetValue(type, out var binder))
            {
                binder(element);
                return;
            }

            var x = Expression.Parameter(typeof(ofElement));
            var y = Expression.Variable(type, nameof(element));

            binder = Expression.Lambda<Action<ofElement>>(
                                    Expression.Block(new[] { y },
                                        Expression.Assign(y, Expression.Convert(x, type)),
                                        BuildBinder(type, y)),
                                    x)
                               .CompileSafe();

            (_binder[type] = binder)(element);
        }

        public static Expression BuildBinder(Type type, Expression element)
        {
            var body = new List<Expression>();

            foreach (var field in type.GetAllFields())
            foreach (var provider in field.GetCustomAttributes().OfType<IBinderFieldProvider>())
                body.Add(ExpressionExtensions.AssignSafe(Expression.Field(element, field), provider.GetValue(element, field)));

            foreach (var method in type.GetAllMethods())
            foreach (var invoker in method.GetCustomAttributes().OfType<IBinderMethodInvoker>())
                body.Add(invoker.Build(element, method));

            return Expression.Block(body);
        }

        static readonly PropertyInfo _elementNodeProperty = typeof(ofElement).GetProperty(nameof(ofElement.Node));

        public static Expression BuildElementNodeAccess(Expression element) => Expression.Property(element, _elementNodeProperty);

        public static Expression BuildContainerInstantiation(Type container, Expression element, Expression key, object initialValue)
        {
            var definition = container.IsGenericType ? container.GetGenericTypeDefinition() : null;

            if (definition != typeof(RefObject<>) && definition != typeof(StateObject<>))
                throw new ArgumentException($"Type {container} is not an instantiable container.");

            var valueType = container.GetGenericArguments()[0];

            var initial = initialValue == null
                ? Expression.Default(valueType) as Expression
                : Expression.Constant(initialValue, valueType);

            return Expression.New(container.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0], BuildElementNodeAccess(element), key, initial);
        }

        public static Expression BuildContainerValueAccess(Expression container) => Expression.Property(container, nameof(RefObject<object>.Current));
    }
}