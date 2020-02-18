using System;
using System.Collections.Generic;
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
    public class EffectAttribute : Attribute, IBinderMethodInvoker
    {
        readonly string[] _depFields = Array.Empty<string>();

        /// <summary>
        /// If true, runs this effect only once on mount and unmount regardless of dependency changes.
        /// </summary>
        public bool Once { get; set; }

        /// <summary>
        /// Creates a new <see cref="EffectAttribute"/>.
        /// </summary>
        public EffectAttribute() { }

        /// <summary>
        /// Creates a new <see cref="EffectAttribute"/> with the given field names as additional dependencies.
        /// </summary>
        /// <param name="dependencyFields">Names of fields to be used as dependencies.</param>
        public EffectAttribute(params string[] dependencyFields)
        {
            if (dependencyFields != null)
                _depFields = dependencyFields;
        }

        static readonly MethodInfo _effectInfoSetMethod = typeof(EffectInfo).GetMethod(nameof(EffectInfo.Set), BindingFlags.Instance | BindingFlags.NonPublic);

        public Expression Build(Expression element, MethodInfo method)
        {
            var type = method.DeclaringType;

            if (method.ReturnType != typeof(void) && method.ReturnType != typeof(EffectCleanupDelegate))
                throw new ArgumentException($"Effect method {method} in {type} returns unsupported type {method.ReturnType}. It must return either void or {nameof(EffectCleanupDelegate)}.");

            // find dependency fields
            var fields = new FieldInfo[_depFields.Length];

            for (var i = 0; i < _depFields.Length; i++)
                fields[i] = type?.GetAllField(_depFields[i]) ?? throw new ArgumentException($"Could not find field '{_depFields[i]}' in {type}.");

            // find parameter providers
            var parameters = method.GetParameters();
            var arguments  = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = parameters[i].GetCustomAttributes().OfType<IBinderMethodParameterProvider>().FirstOrDefault()?.GetValue(element, parameters[i]) ?? throw new ArgumentException($"No provider configured for parameter {parameters} of method {method} in {type}.");

            var body = new List<Expression>();

            // build dependency array
            var deps = Expression.Variable(typeof(object[]), "deps");

            if (Once)
            {
                body.Add(Expression.Assign(deps, Expression.Constant(null, typeof(object[]))));
            }
            else
            {
                var items = new Expression[fields.Length + arguments.Length];

                for (var i = 0; i < fields.Length; i++)
                    items[i] = Expression.Field(element, fields[i]);

                for (var i = 0; i < arguments.Length; i++)
                    items[fields.Length + i] = arguments[i];

                body.Add(Expression.Assign(deps, Expression.NewArrayInit(typeof(object), items.Select(x => Expression.Convert(x, typeof(object))))));
            }

            // create effect info
            var effect = Expression.Variable(typeof(RefObject<EffectInfo>), "effectObj");

            body.Add(Expression.Assign(effect, ElementBinder.BuildContainerInstantiation(typeof(RefObject<EffectInfo>), element, Expression.Constant($"effect:{method.Name}"), null)));

            // call effect set method
            body.Add(Expression.Call(
                Expression.Coalesce(
                    ElementBinder.BuildContainerValueAccess(effect),
                    Expression.Assign(ElementBinder.BuildContainerValueAccess(effect), Expression.New(typeof(EffectInfo)))),
                _effectInfoSetMethod,
                ElementBinder.BuildElementNodeAccess(element),
                Expression.Lambda<EffectDelegate>(buildEffect()),
                deps));

            Expression buildEffect()
            {
                var result = Expression.Call(element, method, arguments);

                if (method.ReturnType == typeof(EffectCleanupDelegate))
                    return result;

                return Expression.Block(result, Expression.Constant(null, typeof(EffectCleanupDelegate)));
            }

            return Expression.Block(new[] { deps, effect }, body);
        }
    }
}