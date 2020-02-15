using NUnit.Framework;

namespace ofreact.Tests
{
    public class RenderingTests
    {
        class Element1 : ofComponent
        {
            public static int Rendered;

            [Prop] public readonly string MyProp;

            public Element1(string myProp = default)
            {
                MyProp = myProp;
            }

            protected override ofElement Render()
            {
                ++Rendered;

                return null;
            }
        }

        [Test]
        public void SkipIfSameProps()
        {
            // use child because root node is programmed to always rerender regardless anything
            var node = new ofRootNode().CreateChild();

            Assert.That(Element1.Rendered, Is.EqualTo(0));

            node.RenderElement(new Element1("test"));

            Assert.That(Element1.Rendered, Is.EqualTo(1));

            node.RenderElement(new Element1("test"));

            Assert.That(Element1.Rendered, Is.EqualTo(1));
        }

        class Element2 : ofComponent
        {
            public static int RenderCount;

            [Prop] public readonly string MyProp;

            public Element2(string myProp = default)
            {
                MyProp = myProp;
            }

            protected override ofElement Render()
            {
                ++RenderCount;

                return null;
            }
        }

        [Test]
        public void RenderIfDifferentProps()
        {
            var node = new ofRootNode().CreateChild();

            node.RenderElement(new Element2("test1"));

            Assert.That(Element2.RenderCount, Is.EqualTo(1));

            node.RenderElement(new Element2("test2"));

            Assert.That(Element2.RenderCount, Is.EqualTo(2));

            node.RenderElement(new Element2("test2"));

            Assert.That(Element2.RenderCount, Is.EqualTo(2));

            node.RenderElement(new Element2("test2"));

            Assert.That(Element2.RenderCount, Is.EqualTo(2));

            node.RenderElement(new Element2("test1"));

            Assert.That(Element2.RenderCount, Is.EqualTo(3));

            node.RenderElement(new Element2("test1"));

            Assert.That(Element2.RenderCount, Is.EqualTo(3));
        }
    }
}