namespace ofreact
{
    public sealed class EffectInfo
    {
        ofElement _element;
        ofNode _node;

        object[] _dependencies;

        EffectDelegate _effect;
        EffectCleanupDelegate _cleanup;

        internal EffectInfo() { }

        public bool Set(ofElement element, EffectDelegate effect, object[] dependencies)
        {
            var pending = _element == null || dependencies?.Length == 0 || !DepsEqual(_dependencies, dependencies);

            _element      = element;
            _node         = element.Node;
            _dependencies = dependencies;
            _effect       = effect;

            if (pending)
                element.Node.Root.PendingEffects.Enqueue(this);

            return pending;
        }

        static bool DepsEqual(object[] a, object[] b)
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

        public void Run()
        {
            Cleanup();

            using (_element.Bind(_node))
                _cleanup = _effect?.Invoke();
        }

        public void Cleanup()
        {
            var cleanup = _cleanup;

            if (cleanup != null)
            {
                _cleanup = null;

                using (_element.Bind(_node))
                    cleanup();
            }
        }
    }
}