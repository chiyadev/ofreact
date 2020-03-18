using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="Game"/> inside a suitable platform-specific <see cref="GameHost"/> and bootstraps the ofreact scene graph.
    /// </summary>
    public class ofGame : ofRootNode
    {
        public GameHost Host { get; }

        /// <summary>
        /// Creates a new <see cref="ofGame"/> with the given host name.
        /// </summary>
        public ofGame(string name) : this(Framework.Host.GetSuitableHost(name)) { }

        /// <summary>
        /// Creates a new <see cref="ofGame"/> using the given host.
        /// </summary>
        public ofGame(GameHost host)
        {
            Host = host;
        }

        public override RenderResult RenderElement(ofElement element)
        {
            using var game = new InternalGame(this, element);

            Host.Run(game);

            return RenderResult.Rendered;
        }

        void RenderInternal(ofElement element) => base.RenderElement(element);

        public override void Dispose()
        {
            base.Dispose();

            Host.Dispose();
        }

        sealed class InternalGame : Game
        {
            readonly ofGame _game;
            readonly ofElement _element;

            public InternalGame(ofGame game, ofElement element)
            {
                _game    = game;
                _element = element;
            }

            DependencyContainer _dependencies;

            protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
                => _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            [Resolved]
            FrameworkConfigManager Config { get; set; }

            [BackgroundDependencyLoader]
            void Load()
            {
                _dependencies.CacheAs<Game>(this);
                _dependencies.Cache(new ofElementBootstrapper.NodeConnector(_game));
            }

            protected override void Update()
            {
                base.Update();

                _game.RenderInternal(new ofPortal(this)
                {
                    new ofContext<GameHost>(value: Host)
                    {
                        new ofContext<FrameworkConfigManager>(value: Config)
                        {
                            _element
                        }
                    }
                });
            }
        }
    }
}