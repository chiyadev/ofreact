namespace ofreact
{
    public sealed class EffectInfo
    {
        public object[] Dependencies;
        public EffectDelegate Callback;

        EffectCleanupDelegate _cleanup;

        internal EffectInfo() { }

        /// <summary>
        /// Returns true if this effect is pending. i.e. the contents of the given dependency array is different from <see cref="Dependencies"/>.
        /// This will always return true if <paramref name="dependencies"/> is an empty array.
        /// </summary>
        public bool IsPending(object[] dependencies)
        {
            static bool objsEqual(object[] a, object[] b)
            {
                if (a == b)
                    return true;

                if (a == null || b == null || a.Length != b.Length)
                    return false;

                for (var i = 0; i < a.Length; i++)
                {
                    var x = a[i];
                    var y = b[i];

                    if (x != y && (x == null || !x.Equals(y)))
                        return false;
                }

                return true;
            }

            return dependencies?.Length == 0 || !objsEqual(Dependencies, dependencies);
        }

        public void Run()
        {
            Cleanup();

            _cleanup = Callback?.Invoke();
        }

        public void Cleanup()
        {
            var cleanup = _cleanup;

            if (cleanup != null)
            {
                _cleanup = null;

                cleanup();
            }
        }
    }
}