using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    public class BoundStateTests
    {
        class Element1 : ofComponent
        {
            public static bool Rendered;

            [State(777)] public readonly StateObject<int> Number;

            protected override ofElement Render()
            {
                Assert.That(Number.Current, Is.EqualTo(777));

                Rendered = true;

                return null;
            }
        }

        [Test]
        public void InitialValue()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element1());

            Assert.That(Element1.Rendered, Is.True);
        }

        class Element2 : ofComponent
        {
            public static int Renders;

            [State] public readonly StateObject<int> Count;

            protected override ofElement Render()
            {
                ++Renders;

                UseEffect(() =>
                {
                    if (Count < 10)
                        ++Count.Current;
                });

                return null;
            }
        }

        [Test]
        public void StateUpdateTriggersRerender()
        {
            using var node = new ofRootNode { AlwaysInvalid = false };

            node.RenderElement(new Element2());

            Assert.That(Element2.Renders, Is.EqualTo(11));
        }

        class Element3 : ofComponent
        {
            public static int ParentRenders;
            public static int NestedRenders1;
            public static int NestedRenders2;

            protected override ofElement Render()
            {
                ++ParentRenders;

                return new Nested1();
            }

            class Nested1 : ofComponent
            {
                [State] public readonly StateObject<int> Count;

                protected override ofElement Render()
                {
                    ++NestedRenders1;

                    UseEffect(() =>
                    {
                        if (Count < 10)
                            ++Count.Current;
                    });

                    return new Nested2();
                }

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        ++NestedRenders2;

                        return null;
                    }
                }
            }
        }

        [Test]
        public void StateUpdateTriggersPartialTreeRerender()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element3());

            Assert.That(Element3.ParentRenders, Is.EqualTo(1));
            Assert.That(Element3.NestedRenders1, Is.EqualTo(11));
            Assert.That(Element3.NestedRenders2, Is.EqualTo(1));
        }
    }
}