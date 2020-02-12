using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    /// <summary>
    /// Marks a method as an effect of an element.
    /// </summary>
    /// <remarks>
    /// Effect methods must return either nothing (void) or an <see cref="EffectCleanupDelegate"/> for cleanup.
    /// Every parameter of the method represents a dependency of the effect, where the effect will be invoked when any of its dependencies changes.
    /// Parameters should be annotated with attributes such as <see cref="RefAttribute"/> or <see cref="StateAttribute"/> that implement <see cref="IElementMethodParameterProvider"/> used for retrieving argument values.
    /// A list of field names can optionally be passed in the attribute constructor as additional dependencies without injecting them as arguments (this is useful when specifying props as dependencies.)
    /// If the effect does not specify any dependencies, it will be invoked on every render; see <see cref="Once"/>.
    /// </remarks>
    public class EffectAttribute : Attribute, IElementMethodInvoker
    {
        readonly string[] _dependencyNames;

        string _name;
        Func<ofElement, object[]> _dependencyBuilder; // this can be null if Once is false!
        Func<ofElement, EffectCleanupDelegate> _effect;

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

        public void Initialize(MethodInfo method, IElementMethodParameterProvider[] parameterProviders)
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
                _dependencyBuilder = GetDependencyBuilder(type, fields, parameterProviders);

            _effect = GetEffectInvoker(type, method, parameterProviders);
        }

        static readonly MethodInfo _parameterProviderGetMethod = typeof(IElementMethodParameterProvider).GetMethod(nameof(IElementMethodParameterProvider.GetValue));

        static Func<ofElement, EffectCleanupDelegate> GetEffectInvoker(Type type, MethodInfo method, IElementMethodParameterProvider[] parameters)
        {
            if (InternalConstants.IsEmitAvailable)
            {
                var element = Expression.Parameter(typeof(ofElement), "element");

                Expression body = Expression.Call(
                    Expression.Convert(element, type),
                    method,
                    parameters.Select(p => Expression.Convert(Expression.Call(Expression.Constant(p), _parameterProviderGetMethod, element), p.Parameter.ParameterType)));

                if (method.ReturnType == typeof(void))
                    body = Expression.Block(body, Expression.Constant(null, typeof(EffectCleanupDelegate)));

                return Expression.Lambda<Func<ofElement, EffectCleanupDelegate>>(body, element).Compile();
            }

            return e =>
            {
                var arguments = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                    arguments[i] = parameters[i].GetValue(e);

                return (EffectCleanupDelegate) method.Invoke(e, arguments);
            };
        }

        static Func<ofElement, object[]> GetDependencyBuilder(Type type, FieldInfo[] fields, IElementMethodParameterProvider[] parameters)
        {
            if (fields.Length == 0 && parameters.Length == 0)
                return e => Array.Empty<object>();

            if (InternalConstants.IsEmitAvailable)
            {
                var element = Expression.Parameter(typeof(ofElement), "element");

                var deps = new Expression[fields.Length + parameters.Length];

                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];

                    Expression expr = Expression.Field(Expression.Convert(element, type), field);

                    if (field.FieldType.IsValueType)
                        expr = Expression.Convert(expr, typeof(object));

                    deps[i] = expr;
                }

                for (var i = 0; i < parameters.Length; i++)
                    deps[fields.Length + i] = Expression.Call(Expression.Constant(parameters[i]), _parameterProviderGetMethod, element);

                return Expression.Lambda<Func<ofElement, object[]>>(
                                      Expression.NewArrayInit(typeof(object), deps),
                                      element)
                                 .Compile();
            }

            return e =>
            {
                var deps = new object[fields.Length + parameters.Length];

                for (var i = 0; i < fields.Length; i++)
                    deps[i] = fields[i].GetValue(e);

                for (var i = 0; i < parameters.Length; i++)
                    deps[fields.Length + i] = parameters[i].GetValue(e);

                return deps;
            };
        }

        public void Invoke(ofElement element)
        {
            var obj    = element.Node.GetNamedRef<EffectInfo>(_name);
            var effect = obj.Current ??= new EffectInfo();

            effect.Set(element, () => _effect(element), _dependencyBuilder?.Invoke(element));
        }
    }
}