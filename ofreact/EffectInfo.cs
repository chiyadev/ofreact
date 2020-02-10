namespace ofreact
{
    public sealed class EffectInfo
    {
        public object[] Dependencies;
        public EffectDelegate Callback;

        EffectCleanupDelegate _cleanup;

        internal EffectInfo() { }

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