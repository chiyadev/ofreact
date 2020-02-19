using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ofreact
{
    public interface IPropProvider
    {
        Expression GetValue(Expression node);
    }

    public class ElementRenderInfo : IPropProvider
    {
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

        public ElementRenderInfo(Type type, ConstructorInfo constructor)
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

            public Expression GetValue(Expression node)
            {
                if (_parameter.HasDefaultValue)
                    return Expression.Constant(_parameter.DefaultValue, _parameter.ParameterType);

                throw new ArgumentException($"Parameter {_parameter} of constructor {_parameter.Member} in {_parameter.Member.DeclaringType} is required but no value was provided.");
            }
        }

        public virtual Expression GetValue(Expression node)
        {
            Expression expr = Expression.New(Constructor, Parameters.Select(p => Props[p.Name].GetValue(node)));

            if (Assignee != null)
                expr = Expression.Assign(Assignee, expr);

            return expr;
        }
    }

    public class FragmentRenderInfo : ElementRenderInfo
    {
        static readonly Type _type = typeof(ofFragment);
        static readonly ConstructorInfo _ctor = _type.GetConstructors()[0];

        public FragmentRenderInfo(IEnumerable<ElementRenderInfo> children) : base(_type, _ctor)
        {
            Props["children"] = new CollectionPropProvider(typeof(IEnumerable<ofElement>), children);
        }
    }
}