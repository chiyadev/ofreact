using System;
using System.Collections.Concurrent;

namespace ofreact
{
    public static class PropEqualityComparer
    {
        /// <summary>
        /// Gets or sets the function used to create a new <see cref="IPropEqualityComparer"/> for a given type.
        /// </summary>
        public static Func<Type, IPropEqualityComparer> Factory { get; set; }

        static PropEqualityComparer()
        {
            // use expression tree comparer if JIT is available
            if (InternalConstants.IsEmitAvailable)
                Factory = t => new ExpressionTreePropEqualityComparer(t);

            // otherwise use reflection-based comparer (slower)
            else
                Factory = t => new DynamicPropEqualityComparer(t);
        }

        /// <summary>
        /// Compares the two given <see cref="ofElement"/> by their fields annotated with <see cref="PropAttribute"/>.
        /// </summary>
        /// <returns>True if all props from both elements are equal.</returns>
        public static bool Equals(ofElement a, ofElement b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            var type = a.GetType();

            if (type != b.GetType())
                return false;

            return GetComparer(type).Equals(a, b);
        }

        static readonly ConcurrentDictionary<Type, IPropEqualityComparer> _comparer = new ConcurrentDictionary<Type, IPropEqualityComparer>();

        public static IPropEqualityComparer GetComparer(Type type)
        {
            if (_comparer.TryGetValue(type, out var comparer))
                return comparer;

            return _comparer[type] = Factory(type);
        }
    }
}