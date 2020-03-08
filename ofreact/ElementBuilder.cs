using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    public interface IPropProvider
    {
        Expression GetValue(ComponentBuilderContext context);
    }

    public class ElementBuilder : IPropProvider
    {
        public sealed class Empty : ElementBuilder
        {
            sealed class EmptyElement : ofElement { }

            static readonly Type _type = typeof(EmptyElement);
            static readonly ConstructorInfo _ctor = _type.GetConstructors()[0];

            public Empty() : base(_type, _ctor) { }

            public override Expression GetValue(ComponentBuilderContext context) => Expression.Constant(null, typeof(ofElement));
        }

        /// <summary>
        /// Type of the element.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructor to use to render this element.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        /// List of parameters of <see cref="Constructor"/>.
        /// </summary>
        public ParameterInfo[] Parameters { get; }

        /// <summary>
        /// Mapping of parameters and argument expressions.
        /// </summary>
        public Dictionary<string, IPropProvider> Props { get; } = new Dictionary<string, IPropProvider>();

        /// <summary>
        /// Expression to which the constructed element will be assigned.
        /// </summary>
        public Expression Assignee { get; set; }

        public ElementBuilder(Type type, ConstructorInfo constructor)
        {
            Type        = type;
            Constructor = constructor;
            Parameters  = constructor.GetParameters();

            foreach (var parameter in Parameters)
                Props[parameter.Name] = new DefaultPropProvider(parameter);
        }

        sealed class DefaultPropProvider : IPropProvider
        {
            readonly ParameterInfo _parameter;

            public DefaultPropProvider(ParameterInfo parameter)
            {
                _parameter = parameter;
            }

            public Expression GetValue(ComponentBuilderContext context)
            {
                if (_parameter.HasDefaultValue)
                {
                    if (_parameter.DefaultValue == null && _parameter.ParameterType.IsValueType)
                        return Expression.Default(_parameter.ParameterType);

                    return Expression.Constant(_parameter.DefaultValue, _parameter.ParameterType);
                }

                throw new ArgumentException($"Parameter {_parameter} of constructor {_parameter.Member} in {_parameter.Member.DeclaringType} is required but no value was provided.");
            }
        }

        public virtual Expression GetValue(ComponentBuilderContext context)
        {
            Expression expr = Expression.New(Constructor, Parameters.Select(p => Props[p.Name].GetValue(context)));

            if (Assignee != null)
                expr = Expression.Assign(Assignee, expr);

            return expr;
        }
    }

    public class FragmentBuilder : ElementBuilder
    {
        static readonly Type _type = typeof(ofFragment);
        static readonly ConstructorInfo _ctor = _type.GetConstructors()[0];

        public FragmentBuilder(IEnumerable<ElementBuilder> children) : base(_type, _ctor)
        {
            Props["children"] = new CollectionPropProvider(typeof(IEnumerable<ofElement>), children);
        }
    }
}