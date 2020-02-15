using System;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a method as an effect of an element.
    /// </summary>
    /// <remarks>
    /// Effect methods must return either nothing (void) or an <see cref="EffectCleanupDelegate"/> for cleanup.
    /// Every parameter of the method represents a dependency of the effect, where the effect will be invoked when any of its dependencies changes.
    /// Parameters should be annotated with attributes such as <see cref="RefAttribute"/> or <see cref="StateAttribute"/> that implement <see cref="IElementMethodArgumentProvider"/> used for retrieving argument values.
    /// A list of field names can optionally be passed in the attribute constructor as additional dependencies without injecting them as arguments (this is useful when specifying props as dependencies.)
    /// If the effect does not specify any dependencies, it will be invoked on every render; see <see cref="Once"/>.
    /// </remarks>
    public class EffectAttribute : Attribute, IElementMethodInvoker
    {
        readonly string[] _dependencyNames;

        string _name;
        ElementDependencyListBuilderDelegate _dependencyBuilder; // this can be null if Once is false!
        ElementMethodInvokerDelegate _invoker;

        bool IElementMethodInvoker.AllowUnknownParameters => false;

        public MethodInfo Method { get; private set; }

        /// <summary>
        /// If true, runs this effect only once on mount and unmount regardless of dependency changes.
        /// </summary>
        public bool Once { get; set; }

        /// <summary>
        /// Creates a new <see cref="EffectAttribute"/>.
        /// </summary>
        public EffectAttribute()
        {
            _dependencyNames = Array.Empty<string>();
        }

        /// <summary>
        /// Creates a new <see cref="EffectAttribute"/> with the given field names as additional dependencies.
        /// </summary>
        /// <param name="dependencyNames">Names of fields to be used as dependencies.</param>
        public EffectAttribute(params string[] dependencyNames)
        {
            _dependencyNames = dependencyNames ?? Array.Empty<string>();
        }

        public void Initialize(MethodInfo method, IElementMethodArgumentProvider[] arguments)
        {
            var type = method.DeclaringType;

            if (method.ReturnType != typeof(void) && method.ReturnType != typeof(EffectCleanupDelegate))
                throw new ArgumentException($"Effect method {method} of {type} returns unsupported type {method.ReturnType}. It must return either void or {nameof(EffectCleanupDelegate)}.");

            Method = method;
            _name  = $"effect:{type}.{method.Name}";

            var fields = new FieldInfo[_dependencyNames.Length];

            for (var i = 0; i < _dependencyNames.Length; i++)
            {
                var field = type?.GetAllField(_dependencyNames[i]);

                if (field == null)
                    throw new ArgumentException($"Could not find field '{_dependencyNames[i]}' in {type}.");

                fields[i] = field;
            }

            if (!Once)
                _dependencyBuilder = InternalReflection.Factory.GetElementDependencyListBuilder(type, fields, arguments);

            _invoker = InternalReflection.Factory.GetElementMethodInvoker(type, method, arguments);
        }

        public void Invoke(ofElement element)
        {
            var obj    = element.Node.GetNamedRef<EffectInfo>(_name);
            var effect = obj.Current ??= new EffectInfo();

            effect.Set(element.Node, () => (EffectCleanupDelegate) _invoker(element), _dependencyBuilder?.Invoke(element));
        }
    }
}