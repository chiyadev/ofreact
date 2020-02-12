using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    public static class ElementAttributeBinder
    {
        public static void Bind(ofElement element)
        {
            var type = element?.GetType();

            if (type == null)
                return;

            GetBinder(type)(element);
        }

        static readonly ConcurrentDictionary<Type, Action<ofElement>> _comparer = new ConcurrentDictionary<Type, Action<ofElement>>();

        static Action<ofElement> GetBinder(Type type)
        {
            if (_comparer.TryGetValue(type, out var comparer))
                return comparer;

            return _comparer[type] = BuildBinder(type);
        }

        static Action<ofElement> BuildBinder(Type type)
        {
            var fieldBinders   = new List<(IElementFieldBinder, Action<ofElement, object> setter)>();
            var methodInvokers = new List<IElementMethodInvoker>();

            foreach (var field in type.GetAllFields())
            foreach (var attribute in field.GetCustomAttributes().OfType<IElementFieldBinder>())
            {
                attribute.Initialize(field);

                fieldBinders.Add((attribute, BuildFieldSetter(type, field)));
            }

            foreach (var method in type.GetAllMethods())
            foreach (var attribute in method.GetCustomAttributes().OfType<IElementMethodInvoker>())
            {
                var parameters         = method.GetParameters();
                var parameterProviders = new IElementMethodParameterProvider[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var provider = parameters[i].GetCustomAttributes().OfType<IElementMethodParameterProvider>().FirstOrDefault();

                    if (provider == null && !attribute.AllowUnknownParameters)
                        throw new ArgumentException($"Cannot find providers for parameter {parameters[i]} of {method} of {type}.");

                    parameterProviders[i] = provider;
                }

                attribute.Initialize(method, parameterProviders);

                methodInvokers.Add(attribute);
            }

            if (fieldBinders.Count == 0 && methodInvokers.Count == 0)
                return e => { };

            var fields  = fieldBinders.ToArray();
            var methods = methodInvokers.ToArray();

            return e =>
            {
                foreach (var (binder, setter) in fields)
                    setter(e, binder.GetValue(e));

                foreach (var method in methods)
                    method.Invoke(e);
            };
        }

        static Action<ofElement, object> BuildFieldSetter(Type type, FieldInfo field)
        {
            if (field.IsLiteral)
                throw new NotSupportedException($"Cannot build setter for constant field {field} of {type}.");

            if (InternalConstants.IsEmitAvailable && !field.IsInitOnly)
            {
                var element = Expression.Parameter(typeof(ofElement), "element");
                var value   = Expression.Parameter(typeof(object), "value");

                return Expression.Lambda<Action<ofElement, object>>(
                                      Expression.Assign(
                                          Expression.Field(
                                              Expression.Convert(element, type),
                                              field),
                                          Expression.Convert(value, field.FieldType)),
                                      element,
                                      value)
                                 .Compile();
            }

            return field.SetValue;
        }
    }
}