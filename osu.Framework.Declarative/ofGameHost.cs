using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="Game"/> inside a suitable platform-specific <see cref="GameHost"/> and bootstraps the ofreact scene graph.
    /// </summary>
    public class ofGameHost : ofRootNode
    {
        public GameHost Host { get; }

        /// <summary>
        /// Creates a new <see cref="ofGameHost"/> with the given host name.
        /// </summary>
        public ofGameHost(string name) : this(Framework.Host.GetSuitableHost(name)) { }

        /// <summary>
        /// Creates a new <see cref="ofGameHost"/> using the given host.
        /// </summary>
        public ofGameHost(GameHost host)
        {
            Host = host;
        }

        public override RenderResult RenderElement(ofElement element)
        {
            using var game = new InternalGame(this, element);

            Host.Run(game);

            return RenderResult.Rendered;
        }

        void RenderElementInternal(ofElement element) => base.RenderElement(element);

        public override void Dispose()
        {
            base.Dispose();

            Host.Dispose();
        }

        sealed class InternalGame : Game
        {
            readonly ofGameHost _root;
            readonly ofElement _element;

            public InternalGame(ofGameHost root, ofElement element)
            {
                _root    = root;
                _element = element;
            }

            DependencyContainer _dependencies;

            protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
                => _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            [BackgroundDependencyLoader]
            void Load()
            {
                _dependencies.CacheAs<Game>(this);
                _dependencies.Cache(new ofElementBootstrapper.NodeConnector(_root));

                Child = new ofElementBootstrapper
                {
                    Element          = _element,
                    RelativeSizeAxes = Axes.Both
                };
            }

            protected override void Update()
            {
                base.Update();

                _root.RenderElementInternal(_element);
            }
        }
    }
}