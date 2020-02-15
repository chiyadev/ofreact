using NUnit.Framework;

namespace ofreact.Tests
{
    public class ComponentTests
    {
        class Element1 : ofComponent
        {
            public static bool DoAlt;

            public static bool OneRendered;
            public static bool TwoRendered;

            protected override ofElement Render()
                => !DoAlt
                    ? new Nested1() as ofElement
                    : new Nested2();

            class Nested1 : ofComponent
            {
                protected override ofElement Render()
                {
                    var value = UseRef("one").Current;

                    Assert.That(value, Is.EqualTo("one"));

                    OneRendered = true;

                    return null;
                }
            }

            class Nested2 : ofComponent
            {
                protected override ofElement Render()
                {
                    // if node was recreated for this element, ref is too
                    var value = UseRef("two").Current;

                    Assert.That(value, Is.EqualTo("two"));

                    TwoRendered = true;

                    return null;
                }
            }
        }

        [Test]
        public void RecreateWrapperNode()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.True);
            Assert.That(Element1.TwoRendered, Is.False);

            Element1.OneRendered = false;
            Element1.DoAlt       = true;

            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.False);
            Assert.That(Element1.TwoRendered, Is.True);

            Element1.TwoRendered = false;
            Element1.DoAlt       = false;

            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.True);
            Assert.That(Element1.TwoRendered, Is.False);
        }

        class Element2 : ofComponent
        {
            public static int Which;
            public static int Rendered;

            protected override ofElement Render()
            {
                if (Which == 0)
                    return null;

                return new Nested();
            }

            class Nested : ofComponent
            {
                protected override ofElement Render()
                {
                    var (state, _) = UseState(Which);

                    Assert.That(state, Is.EqualTo(Which));

                    ++Rendered;

                    return null;
                }
            }
        }

        [Test]
        public void NullClearsChildState()
        {
            using var node = new ofRootNode();

            Element2.Which = 0;

            node.RenderElement(new Element2());

            Assert.That(Element2.Rendered, Is.EqualTo(0));

            Element2.Which = 1;

            node.RenderElement(new Element2());

            Assert.That(Element2.Rendered, Is.EqualTo(1));

            Element2.Which = 0;

            node.RenderElement(new Element2());

            Assert.That(Element2.Rendered, Is.EqualTo(1));

            Element2.Which = 2;

            node.RenderElement(new Element2());

            Assert.That(Element2.Rendered, Is.EqualTo(2));
        }
    }
}