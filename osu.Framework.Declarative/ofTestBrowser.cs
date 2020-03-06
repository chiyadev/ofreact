using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="TestBrowser"/>.
    /// </summary>
    public class ofTestBrowser : ofContainerBase<TestBrowser>
    {
        [Prop] public readonly string AssemblyNamespace;

        /// <summary>
        /// Creates a new <see cref="ofTestBrowser"/>.
        /// </summary>
        public ofTestBrowser(object key = default,
                             RefDelegate<TestBrowser> @ref = default,
                             DrawableStyleDelegate<TestBrowser> style = default,
                             string assemblyNamespace = "") : base(key, @ref, style)
        {
            AssemblyNamespace = assemblyNamespace;
        }

        protected override TestBrowser CreateDrawable() => new InternalTestBrowser(AssemblyNamespace, this);

        sealed class InternalTestBrowser : TestBrowser
        {
            readonly ofTestBrowser _browser;

            public InternalTestBrowser(string assemblyNamespace, ofTestBrowser browser) : base(assemblyNamespace)
            {
                _browser = browser;
            }

            DependencyContainer _dependencies;

            protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
                => _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            [BackgroundDependencyLoader]
            void Load() => _dependencies.Cache(new ofElementBootstrapper.NodeConnector(_browser.Node));
        }
    }
}