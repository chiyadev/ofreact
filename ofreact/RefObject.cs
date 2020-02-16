using System.Collections.Generic;

namespace ofreact
{
    public delegate void RefDelegate<in T>(T value);

    public interface IContainerObject
    {
        object Current { get; }
    }

    /// <summary>
    /// A container that represents a reference to a value.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public sealed class RefObject<T> : IContainerObject
    {
        readonly IDictionary<string, object> _dict;
        readonly string _key;

        /// <summary>
        /// Gets or sets the current value referenced by this object.
        /// </summary>
        public T Current
        {
            get => _dict.TryGetValue(_key, out var value) ? (T) value : default;
            set => _dict[_key] = value;
        }

        object IContainerObject.Current => Current;

        internal RefObject(ofNode node, string key, T initialValue)
        {
            _dict = node.State;

            // - key is always lowercase to achieve case-insensitivity
            // - underscore prefix convention is removed
            _key = key.ToLowerInvariant().TrimStart('_');

            if (!_dict.ContainsKey(_key))
                Current = initialValue;
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