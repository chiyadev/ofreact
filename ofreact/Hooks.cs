using System;
using System.Runtime.CompilerServices;

namespace ofreact
{
    /// <summary>
    /// Defines a set of hook methods that can be used in the rendering function of an <see cref="ofElement"/>.
    /// </summary>
    /// <remarks>
    /// Hooks can be used in conjunction with attribute-bound instance members.
    /// </remarks>
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
        public static RefObject<T> UseRef<T>(T initialValue = default) => ofElement.DefineHook(node => node.GetHookRef(initialValue));

        /// <summary>
        /// Returns a stateful value and a function to update it.
        /// </summary>
        /// <remarks>
        /// The setter function is used to update the state.
        /// It accepts a new state value and enqueues a rerender of this element.
        /// </remarks>
        /// <param name="initialValue">Initial value of the variable.</param>
        /// <typeparam name="T">Type of the variable.</typeparam>
        public static (T, Action<T>) UseState<T>(T initialValue = default) => ofElement.DefineHook<(T, Action<T>)>(node =>
        {
            var obj = node.GetHookRef(initialValue);

            return (obj.Current, value =>
            {
                obj.Current = value;

                node.Invalidate();
            });
        });

        /// <summary>
        /// Accepts a context type and returns the current context value for that type.
        /// </summary>
        /// <remarks>
        /// The current context value is determined by the value prop of the nearest ancestor <see cref="ofContext{TContext}"/> in the tree.
        /// When the context value changes, this hook will enqueue a rerender with the latest context value.
        /// </remarks>
        /// <typeparam name="T">Type of context object.</typeparam>
        public static T UseContext<T>() => ofElement.DefineHook(node =>
        {
            var context = node.FindNearestContext<T>();
            var info    = context.Info;

            UseEffect(() =>
            {
                info?.Subscribe(node);

                return () => info?.Unsubscribe(node);
            }, info);

            return context.Value;
        });

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
        public static void UseEffect(EffectDelegate callback, params object[] dependencies) => ofElement.DefineHook(node =>
        {
            var obj    = node.GetHookRef<EffectInfo>();
            var effect = obj.Current ??= new EffectInfo();

            effect.Set(node, callback, dependencies);
        });

        /// <inheritdoc cref="UseEffect(EffectDelegate,object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UseEffect(Action callback, params object[] dependencies) => UseEffect(() =>
        {
            callback?.Invoke();
            return null;
        }, dependencies);

        /// <summary>
        /// Returns a <see cref="RefObject{T}"/> of <see cref="ofNode"/>s.
        /// This is a helper hook for <see cref="UseRef{T}"/> to hold the child nodes and <see cref="UseEffect(EffectDelegate,object[])"/> to dispose them on unmount.
        /// </summary>
        public static RefObject<ofNode[]> UseChildren()
        {
            var children = UseRef(Array.Empty<ofNode>());

            UseEffect(() => () =>
            {
                var current = children.Current;

                if (current != null)
                    foreach (var child in current)
                        child?.Dispose();

                children.Current = Array.Empty<ofNode>();
            }, null);

            return children;
        }

        /// <inheritdoc cref="UseChildren"/>
        public static RefObject<ofNode> UseChild()
        {
            var obj = UseRef<ofNode>();

            UseEffect(() => () =>
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
        /// Returns a memoized value.
        /// </summary>
        /// <remarks>
        /// Accepts a "create" function and a list of dependencies.
        /// <see cref="UseMemo{T}"/> will only recompute the memoized value when one of the dependencies has changed.
        /// This optimization helps to avoid expensive calculations on every render.
        /// </remarks>
        /// <param name="create">Function to compute the memoized value.</param>
        /// <param name="dependencies">List of dependencies that will cause a recomputation when the values change.</param>
        /// <typeparam name="T">Type of the memoized value.</typeparam>
        /// <returns>The memoized value.</returns>
        public static T UseMemo<T>(Func<T> create, params object[] dependencies) => ofElement.DefineHook(node =>
        {
            var value = node.GetHookRef<T>();
            var deps  = node.GetHookRef<object[]>();

            if (Utils.ObjectsEqual(dependencies, deps))
                return value;

            return value.Current = create();
        });

        /// <inheritdoc cref="UseCallback{T}(T,object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Action UseCallback(Action callback, params object[] dependencies) => UseCallback<Action>(callback, dependencies);

        /// <inheritdoc cref="UseCallback{T}(T,object[])"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<T> UseCallback<T>(Func<T> callback, params object[] dependencies) => UseCallback<Func<T>>(callback, dependencies);

        /// <summary>
        /// Returns a memoized callback.
        /// </summary>
        /// <remarks>
        /// Accepts a callback method or lambda and a list of dependencies.
        /// <see cref="UseCallback{T}(T,object[])"/> will return a memoized version of the given callback that only changes if one of the dependencies has changed.
        /// This is useful when passing callbacks to optimized child elements that rely on reference equality to prevent unnecessary renders (default behavior of all <see cref="ofElement"/>s).
        /// </remarks>
        /// <param name="callback">Callback method or lambda to memoize.</param>
        /// <param name="dependencies">List of dependencies that will cause the new <paramref name="callback"/> to be returned when the values change.</param>
        /// <typeparam name="T">Type of the memoized callback.</typeparam>
        /// <returns>The memoized callback.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T UseCallback<T>(T callback, params object[] dependencies) where T : Delegate => UseMemo(() => callback, dependencies);
    }
}