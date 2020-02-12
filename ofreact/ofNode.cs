using System;
using System.Collections.Generic;

namespace ofreact
{
    /// <summary>
    /// Represents a node in ofreact.
    /// </summary>
    public class ofNode : IDisposable
    {
        /// <summary>
        /// <see cref="ofNode"/> that is the root of the tree.
        /// </summary>
        public ofNodeRoot Root { get; internal set; }

        /// <summary>
        /// <see cref="ofNode"/> that contains this node.
        /// </summary>
        public ofNode Parent { get; }

        /// <summary>
        /// Dictionary of stateful variables.
        /// </summary>
        /// <remarks>
        /// Named states are lowercase string keys.
        /// Hook states are string keys prefixed with ^ (caret character).
        /// </remarks>
        public Dictionary<string, object> State { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the last element bound to this node.
        /// </summary>
        public ofElement Element { get; private set; }

        /// <summary>
        /// Returns true if there is an element bound to this node.
        /// </summary>
        public bool IsBound => Element?.Node == this;

        internal bool Unmounted;
        internal int? HookCount; // used for hook validation

        internal ofNode(ofNode parent)
        {
            Root   = parent?.Root;
            Parent = parent;
        }

        /// <summary>
        /// Unmarks this node from rerender (bind).
        /// </summary>
        /// <returns>True if this was previously marked.</returns>
        public bool Validate(ofElement element)
        {
            if (InternalConstants.ValidateNodeBind)
            {
                if (Unmounted)
                    throw new InvalidOperationException("Cannot validate an unmounted node.");

                if (IsBound)
                    throw new InvalidOperationException("Cannot validate a node already bound by another element.");
            }
            else
            {
                if (Unmounted || IsBound)
                    return false;
            }

            var lastElement = Element;
            Element = element;

            // if marked for rerender
            if (Root.RerenderNodes.Remove(this))
                return true;

            // if props changed
            if (!PropEqualityComparer.Equals(element, lastElement))
                return true;

            return false;
        }

        /// <summary>
        /// Marks this node for rerender.
        /// </summary>
        /// <returns>True if this node was previously unmarked.</returns>
        public bool Invalidate()
        {
            if (Unmounted)
                return false;

            return Root.RerenderNodes.Add(this);
        }

        /// <summary>
        /// Creates an <see cref="ofNode"/> that is a child of this node.
        /// </summary>
        public ofNode CreateChild() => new ofNode(this) { Unmounted = Unmounted };

        /// <summary>
        /// Disposes this node (unmount).
        /// </summary>
        public void Dispose()
        {
            Element?.OnUnmount(this);
            Element = null;

            Unmounted = true;
        }
    }

    /// <summary>
    /// Represents the root node of a declarative tree.
    /// </summary>
    public class ofNodeRoot : ofNode
    {
        /// <summary>
        /// List of context objects.
        /// </summary>
        public Stack<object> Contexts { get; } = new Stack<object>();

        /// <summary>
        /// Set of <see cref="ofNode"/> that are marked for rerender.
        /// </summary>
        public HashSet<ofNode> RerenderNodes { get; } = new HashSet<ofNode>();

        /// <summary>
        /// List of effects to be triggered after render.
        /// </summary>
        public Queue<EffectInfo> PendingEffects { get; } = new Queue<EffectInfo>();

        /// <summary>
        /// Creates a new <see cref="ofNodeRoot"/>.
        /// </summary>
        public ofNodeRoot() : base(null)
        {
            Root = this;
        }

        IEnumerable<ofNode> GetRerenderNodes() // copy to array for iteration
        {
            var array = new ofNode[RerenderNodes.Count];
            RerenderNodes.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Renders the given root element.
        /// </summary>
        /// <param name="element">Element to render.</param>
        public void RenderElement(ofElement element)
        {
            if (!element.MatchNode(this))
                throw new ArgumentException($"Element {element.GetType().FullName} cannot bind to this root node.");

            // render root element
            element.RenderSubtree(this);

            do
            {
                // if there are nodes skipped due to optimization somewhere in the tree, render them too
                foreach (var node in GetRerenderNodes())
                    node.Element?.RenderSubtree(node);

                // run effects
                while (PendingEffects.TryDequeue(out var effect))
                    effect.Run();
            }
            while (RerenderNodes.Count != 0);
        }
    }
}