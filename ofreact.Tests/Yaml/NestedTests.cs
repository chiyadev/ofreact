using NUnit.Framework;
using ofreact.Yaml;

namespace ofreact.Tests.Yaml
{
    public class NestedTests
    {
        [Test]
        public void NestedYaml()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  YamlComponent:
    document: |
      render:
        Fragment:
          children:
            YamlComponent:
              document: |
                render:
                  Fragment:"));

            node.Diagnostics.EnsureRendered(
              typeof(ofYamlComponent),
                typeof(ofYamlComponent),
                typeof(ofFragment),
                typeof(ofYamlComponent),
                typeof(ofFragment));
        }
    }
}