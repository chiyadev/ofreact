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
  Box:
    style:
      name: test
      size: 100 .5
      position: .5
      relativeSizeAxes: y
      relativePositionAxes: x y
      origin: centre
      colour:
        vertical:
          - red
          - blue .3
      alpha: .6");
        }
    }
}