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
    size: .5
    relativeSizeAxes: both
    anchor: centre
    origin: centre
    children:
      - Box:
          colour: white
          relativeSizeAxes: both
      - Box:
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
          text: My test sprite text
          font:
            size: 30
          padding:
            top: 10
            bottom: 10
          colour: 128, 0, 230");
        }
    }
}