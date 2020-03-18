using ofreact;
using ofreact.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osu.Framework.Statistics;

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

            if (IsDiagnosticsEnabled)
                Diagnostics = new GameRenderDiagnostics();
        }

        sealed class GameRenderDiagnostics : RenderDiagnostics
        {
            public readonly GlobalStatistic<int> NodesRenderedStatistic = GlobalStatistics.Get<int>(nameof(ofreact), nameof(NodesRendered));
            public readonly GlobalStatistic<int> NodesSkippedStatistic = GlobalStatistics.Get<int>(nameof(ofreact), nameof(NodesSkipped));
            public readonly GlobalStatistic<int> NodesInvalidatedStatistic = GlobalStatistics.Get<int>(nameof(ofreact), nameof(NodesInvalidated));
            public readonly GlobalStatistic<int> NodesDisposedStatistic = GlobalStatistics.Get<int>(nameof(ofreact), nameof(NodesDisposed));
            public readonly GlobalStatistic<int> EffectsInvokedStatistic = GlobalStatistics.Get<int>(nameof(ofreact), nameof(EffectsInvoked));

            public override void OnNodeRendering(ofNode node)
            {
                base.OnNodeRendering(node);
                NodesRenderedStatistic.Value = NodesRendered.Count;
            }

            public override void OnNodeRenderSkipped(ofNode node)
            {
                base.OnNodeRenderSkipped(node);
                NodesSkippedStatistic.Value = NodesSkipped.Count;
            }

            public override void OnNodeInvalidated(ofNode node)
            {
                base.OnNodeInvalidated(node);
                NodesInvalidatedStatistic.Value = NodesInvalidated.Count;
            }

            public override void OnNodeDisposed(ofNode node)
            {
                base.OnNodeDisposed(node);
                NodesDisposedStatistic.Value = NodesDisposed.Count;
            }

            public override void OnEffectInvoking(EffectInfo effect)
            {
                base.OnEffectInvoking(effect);
                EffectsInvokedStatistic.Value = EffectsInvoked.Count;
            }

            public override void Clear()
            {
                base.Clear();

                NodesRenderedStatistic.Value    = 0;
                NodesSkippedStatistic.Value     = 0;
                NodesInvalidatedStatistic.Value = 0;
                NodesDisposedStatistic.Value    = 0;
                EffectsInvokedStatistic.Value   = 0;
            }
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