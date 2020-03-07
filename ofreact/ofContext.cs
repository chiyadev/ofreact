using System;
using System.Collections.Generic;

namespace ofreact
{
    /// <summary>
    /// Makes a context object available to all descendants of this element.
    /// </summary>
    /// <remarks>
    /// Contexts objects that implement <see cref="IDisposable"/> will be disposed automatically.
    /// </remarks>
    /// <typeparam name="TContext">Type of context object.</typeparam>
    public class ofContext<TContext> : ofFragment
    {
        [Prop] public readonly TContext Value;

        /// <summary>
        /// Creates a new <see cref="ofContext{TContext}"/>.
        /// </summary>
        public ofContext(ElementKey key = default, IEnumerable<ofElement> children = default, TContext value = default) : base(key, children)
        {
            Value = value;
        }

        [Effect(nameof(Value))]
        EffectCleanupDelegate OnValueChanged()
        {
            if (Value is IDisposable disposable)
                return disposable.Dispose;

            return null;
        }

        protected internal override bool RenderSubtree()
        {
            Node.LocalContext.Value = Value;

            return base.RenderSubtree();
        }
    }
}