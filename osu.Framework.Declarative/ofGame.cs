using System.Collections;
using System.Collections.Generic;
using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Platform;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Encapsulates a method that is used to create a new <see cref="GameHost"/>.
    /// </summary>
    public delegate GameHost GameHostFactoryDelegate(string name);

    /// <summary>
    /// Contains variables used in <see cref="GameHost"/> initialization.
    /// </summary>
    public class GameHostOptions
    {
        /// <summary>
        /// True to enable IPC capabilities (inter-process communication).
        /// </summary>
        public bool BindIpc { get; set; }

        public bool PortableInstallation { get; set; }
        public bool UseSdl { get; set; }

        /// <summary>
        /// Creates a new <see cref="GameHost"/> suitable for the calling environment.
        /// </summary>
        public GameHost Create(string name) => Host.GetSuitableHost(name, BindIpc, PortableInstallation, UseSdl);

        public static implicit operator GameHostFactoryDelegate(GameHostOptions args) => args.Create;
    }

    /// <summary>
    /// Renders a <see cref="Game"/> inside a suitable platform-specific game host and bootstraps the ofreact scene graph.
    /// </summary>
    public class ofGame : ofElement, IEnumerable<ofElement>
    {
        [Prop] public readonly GameHostFactoryDelegate Host;
        [Prop] public readonly RefDelegate<Game> GameRef;
        [Prop] public readonly RefDelegate<GameHost> HostRef;
        [Prop] public readonly List<ofElement> Children;

        /// <summary>
        /// Creates a new <see cref="ofContainer"/>.
        /// </summary>
        public ofGame(string name,
                      GameHostFactoryDelegate host = default,
                      RefDelegate<Game> gameRef = default,
                      RefDelegate<GameHost> hostRef = default,
                      IEnumerable<ofElement> children = default) : base(name)
        {
            Host = host ?? new GameHostOptions();

            GameRef = gameRef;
            HostRef = hostRef;

            Children = children == null
                ? new List<ofElement>()
                : new List<ofElement>(children);
        }

        /// <summary>
        /// Adds an element as a child of this game.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(ofElement element) => Children.Add(element);

        protected override bool RenderSubtree()
        {
            if (!base.RenderSubtree())
                return false;

            using (var host = Host(Key.ToString()))
            {
                var game = new Bootstrapper(this);

                HostRef?.Invoke(host);
                GameRef?.Invoke(game);

                host.Run(game);
            }

            HostRef?.Invoke(null);
            GameRef?.Invoke(null);

            return true;
        }

        sealed class Bootstrapper : Game
        {
            readonly ofGame _game;

            public Bootstrapper(ofGame game)
            {
                _game = game;
            }

            DependencyContainer _dependencies;

            protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
                _dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            [BackgroundDependencyLoader]
            void Load()
            {
                _dependencies.CacheAs<Game>(this);
                _dependencies.CacheAs(_game);

                Child = new ofDrawableBootstrapper
                {
                    Element          = _game.Children,
                    RelativeSizeAxes = Axes.Both
                };
            }
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}