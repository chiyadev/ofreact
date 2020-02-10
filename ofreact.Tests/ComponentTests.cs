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
            using var node = new ofNodeRoot();

            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.True);
            Assert.That(Element1.TwoRendered, Is.False);

            Element1.OneRendered = false;
            Element1.DoAlt       = true;

            node.Invalidate();
            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.False);
            Assert.That(Element1.TwoRendered, Is.True);

            Element1.TwoRendered = false;
            Element1.DoAlt       = false;

            node.Invalidate();
            node.RenderElement(new Element1());

            Assert.That(Element1.OneRendered, Is.True);
            Assert.That(Element1.TwoRendered, Is.False);
        }
    }
}