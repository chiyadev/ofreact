using System.Collections.Generic;

namespace ofreact
{
    public delegate void RefDelegate<in T>(T value);

    /// <summary>
    /// A container that represents a reference to a value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public sealed class RefObject<T>
    {
        readonly IDictionary<string, object> _dict;
        readonly string _key;

        /// <summary>
        /// Gets or sets the current value referenced by this object.
        /// </summary>
        public T Current
        {
            get
            {
                lock (_dict)
                    return _dict.TryGetValue(_key, out var value) ? (T) value : default;
            }
            set
            {
                lock (_dict)
                    _dict[_key] = value;
            }
        }

        internal RefObject(ofNode node, string key, T initialValue)
        {
            _dict = node.State;

            // underscore convention is removed
            _key = key.TrimStart('_');

            // initial value
            lock (_dict)
            {
                if (!_dict.ContainsKey(_key))
                    Current = initialValue;
            }
        }

        public override bool Equals(object obj) => obj is RefObject<T> refObj && (Current?.Equals(refObj.Current) ?? false);
        public override int GetHashCode() => Current?.GetHashCode() ?? 0;
        public override string ToString() => Current?.ToString() ?? base.ToString();

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