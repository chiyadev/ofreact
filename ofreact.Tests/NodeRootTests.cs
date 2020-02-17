using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class NodeRootTests
    {
        class ElementTypeA : ofElement { }

        class ElementTypeB : ofElement { }

        [Test]
        public void MismatchingBind()
        {
            var node = new ofRootNode();

            node.RenderElement(new ElementTypeA());

            Assert.That(node.RenderElement(new ElementTypeB()), Is.False);
        }
    }
}