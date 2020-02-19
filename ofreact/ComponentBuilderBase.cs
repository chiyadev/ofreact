using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ofreact
{
    public interface IComponentBuilder
    {
        /// <summary>
        /// Name of this component.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Declares a new variable in the render function.
        /// </summary>
        /// <param name="type">Type of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Initial value of the variable.</param>
        /// <param name="variable">Variable expression.</param>
        bool TryDeclareVariable(Type type, string name, Expression value, out ParameterExpression variable);

        /// <summary>
        /// Returns a variable declared in the render function.
        /// </summary>
        /// <param name="name">Name of the variable, case sensitive.</param>
        ParameterExpression GetVariable(string name);

        /// <summary>
        /// Builds a new <see cref="ofElement"/>.
        /// </summary>
        ofElement Build();

        /// <summary>
        /// Builds a <see cref="FunctionComponent"/> delegate that renders this component.
        /// </summary>
        FunctionComponent BuildRenderer();
    }

    /// <summary>
    /// Defines the base class for building a component dynamically.
    /// </summary>
    public abstract class ComponentBuilderBase : IComponentBuilder
    {
        readonly Dictionary<string, VariableInfo> _variables = new Dictionary<string, VariableInfo>();

        sealed class VariableInfo
        {
            public readonly ParameterExpression Name;
            public readonly Expression Value;

            public VariableInfo(ParameterExpression name, Expression value)
            {
                Name  = name;
                Value = value;
            }
        }

        public string Name { get; set; }

        public bool TryDeclareVariable(Type type, string name, Expression value, out ParameterExpression variable)
        {
            variable = Expression.Variable(type, name);

            return _variables.TryAdd(name, new VariableInfo(variable, value));
        }

        public ParameterExpression GetVariable(string name) => _variables.GetValueOrDefault(name)?.Name;

        public ofElement Build() => ofElement.DefineComponent(BuildRenderer());

        public FunctionComponent BuildRenderer()
        {
            var node = Expression.Parameter(typeof(ofNode), "node");

            // get render info before anything else
            var element = Render(node);

            var body = new List<Expression>();

            // declare and initialize variables step
            var variables = _variables.OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            foreach (var variable in variables)
                body.Add(Expression.Assign(variable.Name, variable.Value));

            // return rendered element step
            body.Add(element?.GetValue(node) ?? Expression.Constant(null, typeof(ofElement)));

            return Expression.Lambda<FunctionComponent>(Expression.Block(variables.Select(v => v.Name), body), node).CompileSafe();
        }

        protected abstract ElementRenderInfo Render(Expression node);
    }
}