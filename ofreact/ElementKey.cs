using System;

namespace ofreact
{
    /// <summary>
    /// Represents a key of an element, which is either a string or a number.
    /// </summary>
    public struct ElementKey : ICloneable, IEquatable<ElementKey>, IComparable<ElementKey>
    {
        readonly object _value;

        /// <summary>
        /// Gets a value indicating whether this key is specified.
        /// </summary>
        public bool HasValue => _value != null;

        /// <summary>
        /// Gets the type of this key, or null if this key is not specified.
        /// </summary>
        public Type Type => _value?.GetType();

        ElementKey(object value)
        {
            _value = value;
        }

        public override bool Equals(object obj) => obj is ElementKey key ? Equals(key) : Equals(_value, obj);

        public bool Equals(ElementKey other) => Equals(_value, other._value);

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public override string ToString() => _value?.ToString();

        public object Clone() => new ElementKey(_value);

        public int CompareTo(ElementKey other)
        {
            // ensure either not null
            if (_value == null)
            {
                if (other._value == null)
                    return 0;

                return -1;
            }

            if (other._value == null)
                return 1;

            // ensure same type
            if (Type != other.Type)
                return Type.GUID.CompareTo(other.Type.GUID);

            // ensure comparable
            if (_value is IComparable a && other._value is IComparable b)
                return a.CompareTo(b);

            throw new Exception($"{Type} not comparable.");
        }

        public static implicit operator ElementKey(string value) => new ElementKey(value);
        public static implicit operator ElementKey(bool value) => new ElementKey(value);
        public static implicit operator ElementKey(byte value) => new ElementKey(value);
        public static implicit operator ElementKey(int value) => new ElementKey(value);
        public static implicit operator ElementKey(long value) => new ElementKey(value);
        public static implicit operator ElementKey(float value) => new ElementKey(value);
        public static implicit operator ElementKey(double value) => new ElementKey(value);

        public static bool operator ==(ElementKey a, ElementKey b) => a.Equals(b);
        public static bool operator !=(ElementKey a, ElementKey b) => !a.Equals(b);
        public static bool operator <(ElementKey a, ElementKey b) => a.CompareTo(b) < 0;
        public static bool operator >(ElementKey a, ElementKey b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElementKey a, ElementKey b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElementKey a, ElementKey b) => a.CompareTo(b) >= 0;
    }
}