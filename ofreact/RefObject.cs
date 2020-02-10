using System;
using System.Collections.Generic;

namespace ofreact
{
    public delegate void RefDelegate<in T>(T value);

    /// <summary>
    /// A container that represents a reference to a value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class RefObject<T>
    {
        readonly List<object> _list;
        readonly int _index;

        /// <summary>
        /// Gets or sets the current value referenced by this object.
        /// </summary>
        public T Current
        {
            get => (T) _list[_index];
            set => _list[_index] = value;
        }

        internal RefObject(ofNode node, int index, T initialValue)
        {
            _list  = node.State;
            _index = index;

            if (index == _list.Count)
                _list.Add(initialValue);

            else if (index > _list.Count)
                throw new IndexOutOfRangeException($"Index of state referenced by {nameof(RefObject<T>)} is larger than the state list of {nameof(ofNode)}.");
        }

        /// <summary>
        /// Returns <see cref="Current"/>.
        /// </summary>
        public static implicit operator T(RefObject<T> obj) => obj.Current;

        /// <summary>
        /// Creates a <see cref="RefDelegate{T}"/> that sets <see cref="Current"/> as the given argument.
        /// </summary>
        public static implicit operator RefDelegate<T>(RefObject<T> obj) => v => obj.Current = v;
    }
}