using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class ComponentTests
    {
        public class RecreateWrapperNode : ofComponent
        {
            public bool DoAlt;

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

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(OneRendered, Is.True);
                Assert.That(TwoRendered, Is.False);

                OneRendered = false;
                DoAlt       = true;

                node.RenderElement(this);

                Assert.That(OneRendered, Is.False);
                Assert.That(TwoRendered, Is.True);

                TwoRendered = false;
                DoAlt       = false;

                node.RenderElement(this);

                Assert.That(OneRendered, Is.True);
                Assert.That(TwoRendered, Is.False);
            }
        }

        public class NullClearsChildState : ofComponent
        {
            public static int State;
            public static int Rendered;

            protected override ofElement Render()
            {
                if (State == 0)
                    return null;

                return new Nested();
            }

            class Nested : ofComponent
            {
                protected override ofElement Render()
                {
                    var (state, _) = UseState(State);

                    Assert.That(state, Is.EqualTo(State));

                    ++Rendered;

                    return null;
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                State = 0;

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(0));

                State = 1;

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(1));

                State = 0;

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(1));

                State = 2;

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(2));
            }
        }
    }
}