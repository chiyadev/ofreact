using ofreact;
using osu.Framework.Graphics;
using osuTK;

namespace osu.Framework.Declarative.VisualTests
{
    public class TestTransform : ofTestScene
    {
        protected override ofElement Render()
        {
            return null;

            return new ofTransform()
            {
                new ofBox(style: new DrawableStyle
                {
                    Size   = new Vector2(100),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                })
            };
        }
    }
}