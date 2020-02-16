using System.Runtime.CompilerServices;
using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Defines the base class for rendering an ofreact scene graph bootstrapped in a <see cref="TestScene"/>.
    /// </summary>
    public abstract class ofTestScene : TestScene
    {
        readonly ofRootNode _node = new ofRootNode();

        /// <summary>
        /// Renders this test scene and returns the rendered element.
        /// </summary>
        protected abstract ofElement Render();

        [BackgroundDependencyLoader, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RenderRoot() => _node.RenderElement(new ofContainerContext(this, children: new[] { ofElement.DefineComponent(Render) }));

        protected override void Update() => RenderRoot();

        protected override void Dispose(bool isDisposing)
        {
            _node.Dispose();

            base.Dispose(isDisposing);
        }
    }
}