using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            var fieldAttrs  = new List<IElementFieldBinder>();
            var methodAttrs = new List<IElementMethodInvoker>();

            foreach (var field in type.GetFields())
            foreach (var attribute in field.GetCustomAttributes().OfType<IElementFieldBinder>())
            {
                attribute.Initialize(field);

                fieldAttrs.Add(attribute);
            }

            foreach (var method in type.GetMethods())
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

                methodAttrs.Add(attribute);
            }

            if (fieldAttrs.Count == 0 && methodAttrs.Count == 0)
                return e => { };

            var fields  = fieldAttrs.ToArray();
            var methods = methodAttrs.ToArray();

            return e =>
            {
                foreach (var binder in fields)
                    binder.Bind(e);

                foreach (var method in methods)
                    method.Invoke(e);
            };
        }
    }
}