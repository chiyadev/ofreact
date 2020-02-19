using System.Runtime.CompilerServices;
using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Bootstraps an ofreact element inside an osu!framework <see cref="Container{Drawable}"/>.
    /// </summary>
    public sealed class ofDrawableBootstrapper : Container<Drawable>
    {
        readonly ofRootNode _node = new ofRootNode();

        /// <summary>
        /// Element to bootstrap.
        /// </summary>
        public ofElement Element { get; set; }

        [BackgroundDependencyLoader, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RenderScene() => _node.RenderElement(new ofPortal(this, children: new[] { Element }));

        protected override void Update() => RenderScene();

        protected override void Dispose(bool isDisposing)
        {
            _node.Dispose();

            base.Dispose(isDisposing);
        }
    }
}