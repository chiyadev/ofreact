using NUnit.Framework;
using osu.Framework.Declarative.Yaml;

namespace ofreact.Tests.Yaml
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void NoRender()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(""));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void EmptyRender()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent("render:"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void EmptyRenderList()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent("render: []"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void EmptyRenderListValues()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  -"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void EmptyRenderListValues2()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  -
  -
  -
  -"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void FlattenedLists()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  - - - - - - Fragment:
  - Fragment:"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent), typeof(ofFragment), typeof(ofFragment), typeof(ofFragment));
        }

        [Test]
        public void EmptyRenderMapping()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent("render: {}"));

            node.Diagnostics.EnsureRendered(typeof(ofYamlComponent));
        }

        [Test]
        public void AbstractElement()
        {
            var node = new ofRootNode();

            Assert.That(() => node.RenderElement(new ofYamlComponent(@"
render:
  Element:")), Throws.InstanceOf<YamlComponentException>()); // ofElement is abstract
        }

        [Test]
        public void BadLowercaseName()
        {
            var node = new ofRootNode();

            Assert.That(() => node.RenderElement(new ofYamlComponent(@"
render:
  fragment:")), Throws.InstanceOf<YamlComponentException>()); // fragment should be titlecased
        }
    }
}