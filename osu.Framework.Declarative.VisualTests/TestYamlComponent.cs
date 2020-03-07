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
      relativeSizeAxes: both
      name: test
      colour: red, 0.2");
        }
    }
}