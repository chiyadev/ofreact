using ofreact;
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
                             string assemblyNamespace = default) : base(key, @ref, style)
        {
            AssemblyNamespace = assemblyNamespace;
        }

        protected override TestBrowser CreateDrawable() => new TestBrowser(AssemblyNamespace ?? "");
    }
}