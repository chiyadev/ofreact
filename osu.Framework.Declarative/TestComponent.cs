using ofreact;
using osu.Framework.Graphics;

namespace osu.Framework.Declarative
{
    public class TestComponent : ofComponent
    {
        protected override ofElement Render()
        {
            var fragment = new ofFragment
            {
                new ofFragment
                {
                    new ofFragment()
                }
            };

            return new ofContainer(style: new ContainerStyle
            {
                Alpha            = 0.3f,
                RelativeSizeAxes = Axes.Both
            })
            {
                new ofContainer
                {
                    new ofContainer
                    {
                        new ofContainer()
                    }
                }
            };
        }
    }
}