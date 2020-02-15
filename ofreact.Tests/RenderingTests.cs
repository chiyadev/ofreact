using NUnit.Framework;

namespace ofreact.Tests
{
    public class RenderingTests
    {
        class Element1 : ofComponent
        {
            [Prop] public readonly string MyProp;

            public Element1(string myProp = default)
            {
                MyProp = myProp;
            }

            bool _rendered;

            protected override ofElement Render()
            {
                Assert.That(_rendered, Is.False);

                _rendered = true;
                return null;
            }
        }

        [Test]
        public void SkipIfSameProps()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element1("test"));
            node.RenderElement(new Element1("test"));
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
            using var node = new ofRootNode();

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