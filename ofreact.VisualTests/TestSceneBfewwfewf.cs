using osu.Framework.Declarative;
using osuTK;
using osuTK.Graphics;

namespace ofreact.VisualTests
{
    public class TestSceneBfewwfewf : ofTestScene
    {
        protected override ofElement Render()
        {
            return new ofBox(style: new DrawableStyle
            {
                Size   = new Vector2(100),
                Colour = Color4.White
            });
        }
    }
}