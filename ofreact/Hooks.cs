using System;

namespace ofreact
{
    public static class Hooks
    {
        /// <summary>
        /// Returns a mutable <see cref="RefObject{T}"/> holding a strongly typed variable that is persisted across renders.
        /// </summary>
        /// <remarks>
        /// This is handy for keeping a mutable value across renders without causing a rerender when updating it.
        /// </remarks>
        /// <param name="initialValue">Initial value of the referenced value.</param>
        /// <typeparam name="T">Type of the referenced value.</typeparam>
        public static RefObject<T> UseRef<T>(T initialValue = default) => ofElement.DefineHook(n => UseRefInternal(n, initialValue));

        internal static RefObject<T> UseRefInternal<T>(ofNode node, T initial)
        {
            if (node.Hooks == null)
                throw new InvalidOperationException($"Cannot use hooks outside the rendering method ({node.Element.GetType()}).");

            return node.GetNamedRef($"^{node.Hooks++}", initial);
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
        public static (T, Action<T>) UseState<T>(T initialValue = default) => ofElement.DefineHook(n => UseStateInternal(n, initialValue));

        internal static (T, Action<T>) UseStateInternal<T>(ofNode node, T initial)
        {
            var obj = UseRefInternal(node, initial);

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
        /// <typeparam name="T">Type of context object.</typeparam>
        public static T UseContext<T>() => ofElement.DefineHook(UseContextInternal<T>);

        internal static T UseContextInternal<T>(ofNode node)
        {
            foreach (var context in node.Root.Contexts)
            {
                if (context is T value)
                    return value;
            }

            return default;
        }

        /// <inheritdoc cref="UseEffect(EffectDelegate,object[])"/>
        public static void UseEffect(Action callback, params object[] dependencies) => ofElement.DefineHook(n => UseEffectInternal(n, callback, dependencies));

        internal static void UseEffectInternal(ofNode node, Action callback, object[] dependencies) => UseEffectInternal(node, () =>
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
        public static void UseEffect(EffectDelegate callback, params object[] dependencies) => ofElement.DefineHook(n => UseEffectInternal(n, callback, dependencies));

        internal static void UseEffectInternal(ofNode node, EffectDelegate callback, object[] dependencies)
        {
            var obj    = UseRefInternal<EffectInfo>(node, null);
            var effect = obj.Current ??= new EffectInfo();

            effect.Set(node, callback, dependencies);
        }

        /// <inheritdoc cref="UseChildren"/>
        public static RefObject<ofNode> UseChild() => ofElement.DefineHook(UseChildInternal);

        internal static RefObject<ofNode> UseChildInternal(ofNode node)
        {
            var obj = UseRefInternal<ofNode>(node, null);

            UseEffectInternal(node, () => () =>
            {
                var current = obj.Current;

                if (current != null)
                {
                    current.Dispose();
                    obj.Current = null;
                }
            }, null);

            return obj;
        }

        /// <summary>
        /// Returns a <see cref="RefObject{T}"/> of <see cref="ofNode"/>s.
        /// This is a helper hook for <see cref="UseRef{T}"/> to hold the child nodes and <see cref="UseEffect(EffectDelegate,object[])"/> to dispose them on unmount.
        /// </summary>
        public static RefObject<ofNode[]> UseChildren() => ofElement.DefineHook(UseChildrenInternal);

        internal static RefObject<ofNode[]> UseChildrenInternal(ofNode node)
        {
            var obj = UseRefInternal(node, Array.Empty<ofNode>());

            UseEffectInternal(node, () => () =>
            {
                var current = obj.Current;

                if (current != null)
                    foreach (var child in current)
                        child?.Dispose();

                obj.Current = Array.Empty<ofNode>();
            }, null);

            return obj;
        }
    }
}