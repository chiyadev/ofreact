using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    public class NodeRootTests
    {
        class Element1 : ofComponent
        {
            protected override ofElement Render() => null;
        }

        class Element2 : ofComponent
        {
            protected override ofElement Render() => null;
        }

        [Test]
        public void MismatchingBind()
        {
            var node = new ofNodeRoot();

            node.RenderElement(new Element1());

            Assert.That(() => node.RenderElement(new Element2()), Throws.InstanceOf<ArgumentException>());
        }
    }
}