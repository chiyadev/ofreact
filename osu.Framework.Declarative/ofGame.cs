using System.Collections;
using System.Collections.Generic;
using ofreact;
using osu.Framework.Allocation;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Renders a <see cref="Game"/> inside a suitable platform-specific game host and bootstraps the ofreact scene graph.
    /// </summary>
    public class ofGame : ofElement, IEnumerable<ofElement>
    {
        [Prop] public readonly GameHostFactoryDelegate Host;
        [Prop] public readonly List<ofElement> Children;

        /// <summary>
        /// Creates a new <see cref="ofContainer"/>.
        /// </summary>
        public ofGame(string name, GameHostFactoryDelegate host = default, IEnumerable<ofElement> children = default) : base(name)
        {
            Host = host ?? new GameHostOptions();

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

            using var host = Host(Key.ToString());

            host.Run(new Bootstrap(Children.ToArray()));

            return true;
        }

        sealed class Bootstrap : Game
        {
            readonly ofRootNode _node = new ofRootNode();
            readonly ofElement[] _children;

            public Bootstrap(ofElement[] children)
            {
                _children = children;
            }

            [BackgroundDependencyLoader]
            void Load() { }

            protected override void Update()
            {
                base.Update();

                _node.RenderElement(new ofContext<ContainerDrawableRenderContext>(children: _children, value: new ContainerDrawableRenderContext(this)));
            }

            protected override void Dispose(bool isDisposing)
            {
                _node.Dispose();

                base.Dispose(isDisposing);
            }
        }

        public IEnumerator<ofElement> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}