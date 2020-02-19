using NUnit.Framework;
using ofreact.Yaml;

namespace ofreact.Tests.Yaml
{
    [TestFixture]
    public class FragmentTests
    {
        [Test]
        public void EmptyFragment()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  Fragment:"));

            node.Diagnostics.EnsureRendered(
                typeof(ofYamlComponent),
                typeof(ofFragment));
        }

        [Test]
        public void EmptyFragmentListInlined()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  - Fragment:"));

            node.Diagnostics.EnsureRendered(
                typeof(ofYamlComponent),
                typeof(ofFragment));
        }

        [Test]
        public void ListFragmentWrapper()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  - Fragment:
  - Fragment:
  - Fragment:"));

            node.Diagnostics.EnsureRendered(
                typeof(ofYamlComponent),
                typeof(ofFragment),
                typeof(ofFragment),
                typeof(ofFragment),
                typeof(ofFragment));
        }

        [Test]
        public void FragmentNested()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  Fragment:
    children:
      Fragment:
        children:
          - Fragment:"));

            node.Diagnostics.EnsureRendered(
                typeof(ofYamlComponent),
                typeof(ofFragment),
                typeof(ofFragment),
                typeof(ofFragment));
        }
    }
}