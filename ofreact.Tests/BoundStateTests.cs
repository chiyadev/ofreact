using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class BoundStateTests
    {
        public class InitialValue : ofComponent
        {
            public bool Rendered;

            [State(777)] public readonly StateObject<int> Number;

            protected override ofElement Render()
            {
                Assert.That(Number.Current, Is.EqualTo(777));

                Rendered = true;

                return null;
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(Rendered, Is.True);
            }
        }

        public class UpdateTriggersRerender : ofComponent
        {
            public int Renders;

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

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(Renders, Is.EqualTo(11));
            }
        }

        public class UpdateTriggersPartialTreeRerender : ofComponent
        {
            public int ParentRenders;
            public static int NestedRenders1;
            public static int NestedRenders2;

            protected override ofElement Render()
            {
                ++ParentRenders;

                return new Nested1();
            }

            class Nested1 : ofComponent
            {
                [State] readonly StateObject<int> _count;

                protected override ofElement Render()
                {
                    ++NestedRenders1;

                    UseEffect(() =>
                    {
                        if (_count < 10)
                            ++_count.Current;
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

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(ParentRenders, Is.EqualTo(1));
                Assert.That(NestedRenders1, Is.EqualTo(11));
                Assert.That(NestedRenders2, Is.EqualTo(1));
            }
        }
    }
}