using ofreact;
using osuTK;
using osuTK.Graphics;
using static osu.Framework.Declarative.Hooks;

namespace osu.Framework.Declarative.VisualTests
{
    public class TestSceneBfewwfewf : ofTestScene
    {
        protected override ofElement Render()
        {
            var game = UseDependency<ofGame>();

            return new ofBox(style: new DrawableStyle
            {
                Size   = new Vector2(100),
                Colour = Color4.White
            });
        }
    }
}