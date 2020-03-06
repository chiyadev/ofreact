using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Bootstraps an ofreact element inside an osu!framework <see cref="Container{Drawable}"/>.
    /// </summary>
    public sealed class ofElementBootstrapper : Container<Drawable>
    {
        /// <summary>
        /// Represents a connection between an ancestor and descendant node that are separated by intermediate <see cref="Drawable"/>s.
        /// </summary>
        /// <remarks>
        /// This class is used by <see cref="ofElementBootstrapper"/> to determine whether to create a root node or a descendant node of a connected ancestor.
        /// <see cref="NodeConnector"/> is to be cached as a dependency in a <see cref="Drawable"/>.
        /// </remarks>
        public class NodeConnector
        {
            /// <summary>
            /// Node to connect to.
            /// </summary>
            public ofNode Node { get; }
 
            /// <summary>
            /// Creates a new <see cref="NodeConnector"/>.
            /// </summary>
            /// <param name="node">Node to which descendants will connect.</param>
            public NodeConnector(ofNode node)
            {
                Node = node;
            }
        }

        ofNode _node;

        /// <summary>
        /// Element to bootstrap.
        /// </summary>
        public ofElement Element { get; set; }

        [BackgroundDependencyLoader]
        void Load(NodeConnector connector) => _node = connector.Node.CreateChild();

        protected override void Update() => _node.RenderElement(new ofPortal(this, children: new[] { Element }));

        protected override void Dispose(bool isDisposing)
        {
            _node.Dispose();

            base.Dispose(isDisposing);
        }
    }
}