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
        public ofRootNode Root { get; internal set; }

        /// <summary>
        /// <see cref="ofNode"/> that contains this node.
        /// </summary>
        public ofNode Parent { get; }

        /// <summary>
        /// Dictionary of stateful variables.
        /// </summary>
        /// <remarks>
        /// Named states are lowercase string keys.
        /// Hook states are string keys prefixed with ^ (caret character) followed by zero-based hook index.
        /// </remarks>
        public Dictionary<string, object> State { get; } = new Dictionary<string, object>(0);

        /// <summary>
        /// List of effects specific to this node.
        /// </summary>
        public HashSet<EffectInfo> LocalEffects { get; } = new HashSet<EffectInfo>(0);

        /// <summary>
        /// Gets the last element bound to this node.
        /// </summary>
        public ofElement Element { get; internal set; }

        internal int? HookCount; // used for hook validation

        internal ofNode(ofNode parent)
        {
            Root   = parent?.Root;
            Parent = parent;
        }

        /// <summary>
        /// Renders the given element.
        /// </summary>
        public virtual bool RenderElement(ofElement element)
        {
            if (!Root.RerenderNodes.Remove(this) && InternalReflection.PropsEqual(element, Element))
                return false;

            using (element.Bind(this, true, true))
                return element.RenderSubtree();
        }

        /// <summary>
        /// Returns a named mutable <see cref="RefObject{T}"/> holding a strongly typed variable that is persisted across renders.
        /// </summary>
        /// <param name="key">Name of the reference.</param>
        /// <param name="initialValue">Initial value of the referenced value.</param>
        /// <typeparam name="T">Type of the referenced value.</typeparam>
        public RefObject<T> GetNamedRef<T>(string key, T initialValue = default) => new RefObject<T>(this, key, initialValue);

        /// <summary>
        /// Marks this node for rerender.
        /// </summary>
        /// <returns>True if this node was previously unmarked.</returns>
        public bool Invalidate() => Root.RerenderNodes.Add(this);

        /// <summary>
        /// Creates an <see cref="ofNode"/> that is a child of this node.
        /// </summary>
        public ofNode CreateChild() => new ofNode(this);

        /// <summary>
        /// Disposes this node.
        /// </summary>
        public virtual void Dispose()
        {
            // do effect cleanups
            foreach (var effect in LocalEffects)
                effect.Cleanup();

            LocalEffects.Clear();
            State.Clear();

            Element = null;
        }
    }

    /// <summary>
    /// Represents the root node of a declarative tree.
    /// </summary>
    public class ofRootNode : ofNode
    {
        /// <summary>
        /// List of context objects.
        /// </summary>
        public Stack<object> Contexts { get; } = new Stack<object>(128);

        /// <summary>
        /// Set of <see cref="ofNode"/> that are marked for rerender.
        /// </summary>
        public HashSet<ofNode> RerenderNodes { get; } = new HashSet<ofNode>(512);

        /// <summary>
        /// List of effects to be triggered after render.
        /// </summary>
        public Queue<EffectInfo> PendingEffects { get; } = new Queue<EffectInfo>(1024);

        /// <summary>
        /// Creates a new <see cref="ofRootNode"/>.
        /// </summary>
        public ofRootNode() : base(null)
        {
            Root = this;
        }

        bool GetRerenderNodes(out ofNode[] nodes)
        {
            var count = RerenderNodes.Count;

            if (count == 0)
            {
                nodes = null;
                return false;
            }

            nodes = new ofNode[count];
            RerenderNodes.CopyTo(nodes);

            return true;
        }

        /// <summary>
        /// Renders the given root element.
        /// </summary>
        /// <param name="element">Element to render.</param>
        public override bool RenderElement(ofElement element)
        {
            base.RenderElement(element);

            do
            {
                // if there are nodes skipped due to optimization somewhere in the tree, render them too
                if (GetRerenderNodes(out var nodes))
                    foreach (var node in nodes)
                        node.RenderElement(node.Element);

                // run effects
                while (PendingEffects.TryDequeue(out var effect))
                    effect.Run();
            }
            while (RerenderNodes.Count != 0);

            return true;
        }
    }
}