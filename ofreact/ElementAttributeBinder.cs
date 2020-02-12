using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var fields = new List<IElementFieldBinder>();

            foreach (var field in type.GetFields())
            foreach (var attribute in field.GetCustomAttributes())
            {
                if (attribute is IElementFieldBinder fieldBinder)
                {
                    fieldBinder.Initialize(field);

                    fields.Add(fieldBinder);
                }
            }

            if (fields.Count == 0)
                return e => { };

            var fieldBinders = fields.ToArray();

            return e =>
            {
                foreach (var binder in fieldBinders)
                    binder.Bind(e);
            };
        }
    }
}