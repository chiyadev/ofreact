using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;

namespace ofreact
{
    public interface IComponentBuilder
    {
        /// <summary>
        /// If true, enables full analysis of the component being built. Otherwise, fails quickly on the first exception.
        /// </summary>
        /// <remarks>
        /// If multiple exceptions are thrown in full analysis, they are wrapped in <see cref="AggregateException"/>.
        /// </remarks>
        bool FullAnalysis { get; set; }

        /// <summary>
        /// Builds this component as an <see cref="ofElement"/>.
        /// </summary>
        ofElement Build();

        /// <summary>
        /// Builds an <see cref="FunctionComponent"/> delegate that renders this component.
        /// </summary>
        FunctionComponent BuildRenderer();

        /// <summary>
        /// Builds a <see cref="LambdaExpression"/> that represents the rendering function.
        /// </summary>
        Expression<FunctionComponent> BuildRendererExpression();
    }

    /// <summary>
    /// Defines the base class for building a component dynamically.
    /// </summary>
    public abstract class ComponentBuilderBase : IComponentBuilder
    {
        public bool FullAnalysis { get; set; }

        public ofElement Build() => ofElement.DefineComponent(BuildRenderer());
        public FunctionComponent BuildRenderer() => BuildRendererExpression().CompileSafe();

        public Expression<FunctionComponent> BuildRendererExpression()
        {
            var context = new ComponentBuilderContext(this);
            var element = null as ElementBuilder;

            try
            {
                // build element to be rendered
                element = Render(context);
            }
            catch (Exception e)
            {
                context.OnException(e);
            }

            context.ThrowExceptions();

            var body = new List<Expression>();

            // declare and initialize variables step
            var variables = context.Variables.OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            foreach (var variable in variables)
                body.Add(Expression.Assign(variable.Name, variable.Value));

            // return rendered element step
            body.Add(element?.GetValue(context) ?? Expression.Constant(null, typeof(ofElement)));

            return Expression.Lambda<FunctionComponent>(Expression.Block(variables.Select(v => v.Name), body), context.Node);
        }

        protected abstract ElementBuilder Render(ComponentBuilderContext context);
    }

    public sealed class ComponentBuilderContext
    {
        /// <summary>
        /// Name of the component being built.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Expression for <see cref="ofNode"/> parameter of the render function.
        /// </summary>
        public ParameterExpression Node { get; } = Expression.Parameter(typeof(ofNode), "node");

        /// <summary>
        /// Builder object.
        /// </summary>
        public IComponentBuilder Builder { get; }

        internal ComponentBuilderContext(IComponentBuilder builder)
        {
            Builder = builder;
        }

        internal readonly Dictionary<string, VariableInfo> Variables = new Dictionary<string, VariableInfo>();

        internal sealed class VariableInfo
        {
            public readonly ParameterExpression Name;
            public readonly Expression Value;

            public VariableInfo(ParameterExpression name, Expression value)
            {
                Name  = name;
                Value = value;
            }
        }

        /// <summary>
        /// Declares a new variable.
        /// </summary>
        /// <param name="type">Type of the variable.</param>
        /// <param name="name">Name of the variable.</param>
        /// <param name="value">Initial value of the variable.</param>
        /// <param name="variable">Variable expression.</param>
        public bool TryDeclareVariable(Type type, string name, object value, out ParameterExpression variable)
        {
            variable = Expression.Variable(type, name);

            return Variables.TryAdd(name, new VariableInfo(variable, value as Expression ?? Expression.Constant(value, type)));
        }

        /// <summary>
        /// Returns a variable previously declared.
        /// </summary>
        /// <param name="name">Name of the variable, case sensitive.</param>
        public ParameterExpression GetVariable(string name) => Variables.GetValueOrDefault(name)?.Name;

        readonly List<Exception> _exceptions = new List<Exception>(1);

        public void OnException(Exception e)
        {
            if (e is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions)
                    OnException(inner);

                return;
            }

            _exceptions.Add(e);

            if (!Builder.FullAnalysis)
                ThrowExceptions();
        }

        internal void ThrowExceptions()
        {
            switch (_exceptions.Count)
            {
                case 0:
                    return;

                case 1:
                    var exception = _exceptions[0];

                    _exceptions.Clear();

                    ExceptionDispatchInfo.Capture(exception).Throw();
                    return;

                default:
                    var exceptions = _exceptions.ToArray();

                    _exceptions.Clear();

                    throw new AggregateException(exceptions);
            }
        }
    }
}