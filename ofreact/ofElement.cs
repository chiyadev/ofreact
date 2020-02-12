using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ofreact
{
    /// <summary>
    /// Encapsulates an effect method that returns a cleanup method.
    /// </summary>
    public delegate EffectCleanupDelegate EffectDelegate();

    /// <summary>
    /// Encapsulates the cleanup method of an effect hook.
    /// </summary>
    public delegate void EffectCleanupDelegate();

    /// <summary>
    /// Represents an element in ofreact.
    /// </summary>
    /// <remarks>
    /// Elements are lightweight classes that are created every render.
    /// During render, elements are bound to their respective <see cref="ofNode"/> that hold stateful data across renders.
    /// </remarks>
    public abstract class ofElement : IEquatable<ofElement>
    {
        [ThreadStatic] static ofElement _currentElement;

        /// <summary>
        /// Key used to differentiate this element amongst other sibling elements.
        /// </summary>
        [Prop] public readonly object Key;

        /// <summary>
        /// Creates a new <see cref="ofElement"/>.
        /// </summary>
        protected ofElement(object key = default)
        {
            Key = key;
        }

        /// <summary>
        /// Gets the backing <see cref="ofNode"/> that this element is bound to during render.
        /// </summary>
        public ofNode Node { get; private set; }

        /// <summary>
        /// Binds to the given node.
        /// </summary>
        /// <param name="node">Node to bind to.</param>
        /// <param name="attributes">If true, do attribute-based instance member binding.</param>
        /// <param name="hooks">If true, allow hooks within this scope.</param>
        /// <returns>A value that will unbind when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindScope Bind(ofNode node, bool attributes = false, bool hooks = false) => new BindScope(this, node, attributes, hooks);

        /// <summary>
        /// Represents the scope in which an element is bound to a node.
        /// </summary>
        public readonly ref struct BindScope
        {
            readonly ofElement _current;
            readonly ofElement _last;

            internal BindScope(ofElement element, ofNode node, bool attributes, bool hooks)
            {
                if (element.Node != null)
                    throw new InvalidOperationException("Element is already bound to another node.");

                if (node.IsUnmounted)
                    throw new InvalidOperationException("Cannot bind to an unmounted node.");

                if (node.Element != null && !node.Element.Equals(element))
                    throw new ArgumentException($"Cannot bind element {element} to a node previously bound to element {node.Element}.");

                element.Node = node;
                node.Element = element;

                _last    = _currentElement;
                _current = _currentElement = element;

                if (hooks)
                    element._hooks = 0;

                if (attributes)
                    try
                    {
                        ElementAttributeBinder.Bind(element);
                    }
                    catch
                    {
                        DisposeInternal();
                        throw;
                    }
            }

            /// <summary>
            /// Unbinds the element from the node.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                try
                {
                    if (_current._hooks != null && InternalConstants.ValidateHooks)
                    {
                        var node = _current.Node;

                        if (node.HookCount == null)
                            node.HookCount = _current._hooks;

                        else if (node.HookCount != _current._hooks)
                            throw new InvalidOperationException($"The number of hooks ({_current._hooks}) does not match with the previous render ({node.HookCount}). " +
                                                                "See https://reactjs.org/docs/hooks-rules.html for rules about hooks.");
                    }
                }
                finally
                {
                    DisposeInternal();
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void DisposeInternal()
            {
                _current.Node   = null;
                _current._hooks = null;

                _currentElement = _last;
            }
        }

        /// <summary>
        /// Renders this element.
        /// </summary>
        /// <returns>False to short-circuit the rendering.</returns>
        protected internal virtual bool RenderSubtree() => true;

#region Hooks

        /// <inheritdoc cref="DefineHook{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DefineHook(Action<ofNode> hook) => hook(_currentElement?.Node);

        /// <summary>
        /// Invokes the given callback delegate with <see cref="ofNode"/> of the element currently being rendered by the calling thread.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DefineHook<T>(Func<ofNode, T> hook) => hook(_currentElement?.Node);

        int? _hooks;

        /// <summary>
        /// Returns a mutable <see cref="RefObject{T}"/> holding a strongly typed variable that is persisted across renders.
        /// </summary>
        /// <remarks>
        /// This is handy for keeping a mutable value across renders without causing a rerender when updating it.
        /// </remarks>
        /// <param name="initialValue">Initial value of the referenced value.</param>
        /// <typeparam name="T">Type of the referenced value.</typeparam>
        protected RefObject<T> UseRef<T>(T initialValue = default)
        {
            if (_hooks == null)
                throw new InvalidOperationException($"Cannot use hooks outside the rendering method ({GetType()}).");

            return Node.GetNamedRef($"^{_hooks++}", initialValue);
        }

        /// <summary>
        /// Returns a stateful value and a function to update it.
        /// </summary>
        /// <remarks>
        /// The setter function is used to update the state.
        /// It accepts a new state value and enqueues a rerender of this element.
        /// </remarks>
        /// <param name="initialValue">Initial value of the variable.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        protected (T, Action<T>) UseState<T>(T initialValue = default)
        {
            var obj  = UseRef(initialValue);
            var node = Node;

            return (obj.Current, value =>
            {
                obj.Current = value;

                node.Invalidate();
            });
        }

        /// <summary>
        /// Accepts a context type and returns the current context value for that type.
        /// </summary>
        /// <remarks>
        /// The current context value is determined by the value prop of the nearest <see cref="ofContext{TContext}"/> above this element in the tree.
        /// When the nearest <see cref="ofContext{TContext}"/> above the element updates, this hook will trigger a rerender with the latest context value.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T UseContext<T>()
        {
            foreach (var context in Node.Root.Contexts)
            {
                if (context is T value)
                    return value;
            }

            return default;
        }

        /// <inheritdoc cref="UseEffect(EffectDelegate,object[])"/>
        protected void UseEffect(Action callback, params object[] dependencies) => UseEffect(() =>
        {
            callback?.Invoke();
            return null;
        }, dependencies);

        /// <summary>
        /// Accepts a function that contains imperative, possibly effectful code.
        /// </summary>
        /// <remarks>
        /// Mutations, subscriptions, timers, logging, and other side effects are discouraged inside the render method of an element.
        /// Instead, use this hook for side effects. The callback function will run after rendering is completed.
        /// </remarks>
        /// <param name="callback">Callback function to be triggered, returning a cleanup function.</param>
        /// <param name="dependencies">
        /// List of dependencies that will cause a rerender when the values change.
        /// Conceptually, these are passed as arguments to the callback function.
        /// An empty list will trigger this effect on every render (equivalent to undefined), whereas null will trigger this effect only once on mount (equivalent to []).
        /// </param>
        protected void UseEffect(EffectDelegate callback, params object[] dependencies)
        {
            var obj    = UseRef<EffectInfo>();
            var effect = obj.Current ??= new EffectInfo();

            effect.Set(this, callback, dependencies);
        }

        /// <inheritdoc cref="UseChildren"/>
        protected RefObject<ofNode> UseChild()
        {
            var obj = UseRef<ofNode>();

            obj.Current ??= Node.CreateChild();

            UseEffect(() => () => obj.Current?.Dispose(), null);

            return obj;
        }

        /// <summary>
        /// Returns a <see cref="RefObject{T}"/> of <see cref="ofNode"/>s.
        /// This is a helper hook for <see cref="UseRef{T}"/> to hold the child nodes and <see cref="UseEffect(EffectDelegate,object[])"/> to dispose them on unmount.
        /// </summary>
        protected RefObject<ofNode[]> UseChildren()
        {
            var obj = UseRef(Array.Empty<ofNode>());

            UseEffect(() => () =>
            {
                if (obj.Current != null)
                    foreach (var node in obj.Current)
                        node?.Dispose();
            }, null);

            return obj;
        }

#endregion

#region Override

        /// <summary>
        /// Determines whether the specified object instance is equivalent to this element.
        /// </summary>
        public sealed override bool Equals(object obj) => obj is ofElement e && Equals(e);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ofElement other) => other != null &&
                                               GetType() == other.GetType() &&
                                               KeysEqual(this, other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool KeysEqual(ofElement a, ofElement b) => a.Key == b.Key || a.Key != null && a.Key.Equals(b.Key);

        /// <summary>
        /// Calculates the hash code of this element.
        /// </summary>
        public sealed override int GetHashCode() => HashCode.Combine(this);

        /// <summary>
        /// Returns a string that describes this element.
        /// </summary>
        public sealed override string ToString()
        {
            var str = GetType().FullName;

            if (Key != null)
                str = $"{str} key='{Key}'";

            return str;
        }

#endregion

        public static implicit operator ofElement(ofElement[] x) => (ofFragment) x;
        public static implicit operator ofElement(List<ofElement> x) => (ofFragment) x;
    }
}