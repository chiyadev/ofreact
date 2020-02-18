namespace ofreact
{
    /// <summary>
    /// A container that represents a reference to a value, enqueueing a rerender of the stateful element when it is updated.
    /// </summary>
    /// <remarks>
    /// Internally, this object is a simple wrapper of <see cref="RefObject{T}"/> with a setter that invalidates the current element's <see cref="ofNode"/>.
    /// </remarks>
    /// <typeparam name="T">Type of the value.</typeparam>
    public sealed class StateObject<T>
    {
        readonly ofNode _node;
        readonly RefObject<T> _ref;

        /// <summary>
        /// Gets or sets the current stateful value.
        /// </summary>
        /// <remarks>
        /// Setting a value to this property will enqueue a rerender of the stateful element.
        /// </remarks>
        public T Current
        {
            get => _ref.Current;
            set
            {
                _ref.Current = value;

                _node.Invalidate();
            }
        }

        internal StateObject(ofNode node, string key, T initialValue)
        {
            _node = node;
            _ref  = new RefObject<T>(node, key, initialValue);
        }

        public override bool Equals(object obj) => obj is StateObject<T> stateObj && _ref.Equals(stateObj._ref);
        public override int GetHashCode() => _ref.GetHashCode();
        public override string ToString() => _ref.ToString();

        /// <summary>
        /// Returns <see cref="Current"/>.
        /// </summary>
        public static implicit operator T(StateObject<T> obj) => obj.Current;

        /// <summary>
        /// Returns the underlying <see cref="RefObject{T}"/> of the given <see cref="StateObject{T}"/>.
        /// </summary>
        public static explicit operator RefObject<T>(StateObject<T> obj) => obj._ref;

        /// <summary>
        /// Creates a <see cref="RefDelegate{T}"/> that sets <see cref="Current"/> as the given argument.
        /// </summary>
        public static implicit operator RefDelegate<T>(StateObject<T> obj) => v => obj.Current = v;
    }
}