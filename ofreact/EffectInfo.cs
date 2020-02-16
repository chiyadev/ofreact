using System.Runtime.CompilerServices;

namespace ofreact
{
    public sealed class EffectInfo
    {
        ofNode _node;
        object[] _dependencies;

        EffectDelegate _effect;
        EffectCleanupDelegate _cleanup;

        internal EffectInfo() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Set(ofNode node, EffectDelegate effect, object[] dependencies)
        {
            var pending = _node == null || dependencies?.Length == 0 || !DepsEqual(_dependencies, dependencies);

            _node         = node;
            _dependencies = dependencies;
            _effect       = effect;

            node.LocalEffects.Add(this);

            if (pending)
                node.Root.PendingEffects.Enqueue(this);

            return pending;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke()
        {
            Cleanup();

            using (_node.Element.Bind(_node))
                _cleanup = _effect?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cleanup()
        {
            var cleanup = _cleanup;

            if (cleanup == null)
                return;

            _cleanup = null;

            using (_node.Element.Bind(_node))
                cleanup();
        }
    }
}