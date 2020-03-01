using ofreact;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace osu.Framework.Declarative
{
    /// <summary>
    /// Defines the base class for rendering an ofreact scene graph bootstrapped in a <see cref="TestScene"/>.
    /// </summary>
    public abstract class ofTestScene : TestScene
    {
        /// <summary>
        /// Renders this test scene and returns the rendered element.
        /// </summary>
        protected abstract ofElement Render();

        [BackgroundDependencyLoader]
        void Load() => Child = new ofElementBootstrapper
        {
            Element          = ofElement.DefineComponent(n => Render()),
            RelativeSizeAxes = Axes.Both
        };
    }
}