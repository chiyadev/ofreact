using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public abstract class ofElement
    {
        [ThreadStatic] static ofElement _currentElement;

        static bool? _validateHooks;

        /// <summary>
        /// Gets or sets a value indicating whether hook validation is enabled or not.
        /// This is enabled by default if <see cref="Debugger.IsAttached"/> is true.
        /// </summary>
        public static bool IsHookValidated
        {
            get => _validateHooks ?? Debugger.IsAttached;
            set => _validateHooks = value;
        }

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
        /// <param name="bindAttributes">True to bind attribute-bound members to node.</param>
        /// <returns>A value that will unbind when disposed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindScope Bind(ofNode node, bool bindAttributes = true) => new BindScope(this, node, bindAttributes);

        /// <summary>
        /// Represents the scope in which an element is bound to a node.
        /// </summary>
        public readonly ref struct BindScope
        {
            readonly ofElement _current;
            readonly ofElement _last;

            internal BindScope(ofElement element, ofNode node, bool attrs)
            {
                if (element.Node != null)
                    throw new InvalidOperationException("Element is already bound to another node.");

                element.Node = node;
                node.Element = element;

                _last    = _currentElement;
                _current = _currentElement = element;

                if (attrs)
                    try
                    {
                        InternalReflection.BindElement(element);
                    }
                    catch
                    {
                        Dispose();
                        throw;
                    }
            }

            /// <summary>
            /// Unbinds the element from the node.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose()
            {
                _current.Node = null;

                _currentElement = _last;
            }
        }

        /// <summary>
        /// Renders this element.
        /// </summary>
        /// <returns>False to short-circuit the rendering.</returns>
        protected internal virtual bool RenderSubtree() => true;

        /// <inheritdoc cref="DefineComponent(System.Func{ofNode,ofElement})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ofElement DefineComponent(Func<ofElement> render) => DefineComponent(n => render?.Invoke());

        /// <summary>
        /// Wraps the given rendering function in an <see cref="ofElement"/>.
        /// </summary>
        /// <param name="render">Rendering function.</param>
        /// <returns>An <see cref="ofElement"/> that invokes <paramref name="render"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ofElement DefineComponent(Func<ofNode, ofElement> render) => new FunctionalComponent(render);

        sealed class FunctionalComponent : ofComponent
        {
            readonly Func<ofNode, ofElement> _render;

            public FunctionalComponent(Func<ofNode, ofElement> render)
            {
                _render = render;
            }

            protected override ofElement Render() => _render?.Invoke(Node);
        }

#region Hooks

        /// <inheritdoc cref="DefineHook{T}"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DefineHook(Action<ofNode> hook) => hook(_currentElement?.Node);

        /// <summary>
        /// Invokes the given callback delegate with <see cref="ofNode"/> of the element currently being rendered by the calling thread.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DefineHook<T>(Func<ofNode, T> hook) => hook(_currentElement?.Node);

        /// <inheritdoc cref="Hooks.UseRef{T}"/>
        protected RefObject<T> UseRef<T>(T initialValue = default) => Node.GetHookRef(initialValue);

        /// <inheritdoc cref="Hooks.UseState{T}"/>
        protected (T, Action<T>) UseState<T>(T initialValue = default) => Hooks.UseStateInternal(Node, initialValue);

        /// <inheritdoc cref="Hooks.UseContext{T}"/>
        protected T UseContext<T>() => Hooks.UseContextInternal<T>(Node);

        /// <inheritdoc cref="Hooks.UseEffect(EffectDelegate,object[])"/>
        protected void UseEffect(EffectDelegate callback, params object[] dependencies) => Hooks.UseEffectInternal(Node, callback, dependencies);

        /// <inheritdoc cref="Hooks.UseEffect(Action,object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void UseEffect(Action callback, params object[] dependencies) => UseEffect(() =>
        {
            callback?.Invoke();
            return null;
        }, dependencies);

        /// <inheritdoc cref="Hooks.UseChildren"/>
        protected RefObject<ofNode[]> UseChildren() => Hooks.UseChildrenInternal(Node);

        /// <inheritdoc cref="Hooks.UseChild"/>
        protected RefObject<ofNode> UseChild() => Hooks.UseChildInternal(Node);

#endregion

#region Override

        /// <summary>
        /// Determines whether the specified object instance is equivalent to this element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override bool Equals(object obj) => ReferenceEquals(this, obj);

        /// <summary>
        /// Calculates the hash code of this element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

        /// <summary>
        /// Returns a string that describes this element.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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