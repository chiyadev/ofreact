using ofreact;
using osu.Framework.Declarative.Yaml;

namespace osu.Framework.Declarative.VisualTests
{
    public class TestYamlComponent : ofTestScene
    {
        protected override ofElement Render()
        {
            return new ofYamlComponent(@"
render:
  Container:
    key: test
    style:
      size: .5
      relativeSizeAxes: both
      anchor: centre
      origin: centre
    children:
      - Box:
          style:
            colour: white
            relativeSizeAxes: both
      - Box:
          style:
            name: test
            size: 170, .5
            position: .5
            relativeSizeAxes: y
            relativePositionAxes: x, y
            origin: centre
            colour:
              vertical:
                - red
                - blue, .3
            alpha: .6
      - SpriteText:
          style:
            colour: black");
        }
    }
}