using System;
using System.Linq;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Implements <see cref="IReflectionFactory"/> using reflection.
    /// </summary>
    public class DynamicReflectionFactory : IReflectionFactory
    {
        public static readonly IReflectionFactory Instance = new DynamicReflectionFactory();

        DynamicReflectionFactory() { }

        public PropsEqualityComparerDelegate GetPropsEqualityComparer(Type type)
        {
            var fields = type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)).ToArray();

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
}