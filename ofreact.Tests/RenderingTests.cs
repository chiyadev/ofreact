using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class RenderingTests
    {
        class SkipIfSamePropsElement : ofComponent
        {
            public static int Rendered;

            [Prop] public readonly string MyProp;

            public SkipIfSamePropsElement(string myProp = default)
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
            var node = new ofRootNode { AlwaysInvalid = false }; // we don't want optimization

            Assert.That(SkipIfSamePropsElement.Rendered, Is.EqualTo(0));

            node.RenderElement(new SkipIfSamePropsElement("test"));

            Assert.That(SkipIfSamePropsElement.Rendered, Is.EqualTo(1));

            node.RenderElement(new SkipIfSamePropsElement("test"));

            Assert.That(SkipIfSamePropsElement.Rendered, Is.EqualTo(1));
        }

        class RenderIfDifferentPropsElement : ofComponent
        {
            public static int RenderCount;

            [Prop] public readonly string MyProp;

            public RenderIfDifferentPropsElement(string myProp = default)
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
            var node = new ofRootNode { AlwaysInvalid = false }; // we don't want optimization

            node.RenderElement(new RenderIfDifferentPropsElement("test1"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(1));

            node.RenderElement(new RenderIfDifferentPropsElement("test2"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(2));

            node.RenderElement(new RenderIfDifferentPropsElement("test2"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(2));

            node.RenderElement(new RenderIfDifferentPropsElement("test2"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(2));

            node.RenderElement(new RenderIfDifferentPropsElement("test1"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(3));

            node.RenderElement(new RenderIfDifferentPropsElement("test1"));

            Assert.That(RenderIfDifferentPropsElement.RenderCount, Is.EqualTo(3));
        }
    }
}