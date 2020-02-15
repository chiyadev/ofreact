using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    public class NodeRootTests
    {
        class Element1 : ofElement { }

        class Element2 : ofElement { }

        [Test]
        public void MismatchingBind()
        {
            var node = new ofRootNode();

            node.RenderElement(new Element1());

            Assert.That(() => node.RenderElement(new Element2()), Throws.InstanceOf<ArgumentException>());
        }
    }
}