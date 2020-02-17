using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class UseStateTests
    {
        public class StateUpdateTriggersRerender : ofComponent
        {
            public int RenderCount;

            protected override ofElement Render()
            {
                ++RenderCount;

                var (count, setCount) = UseState(0);

                UseEffect(() =>
                {
                    if (count < 10)
                        setCount(count + 1);
                });

                return null;
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(RenderCount, Is.EqualTo(11));
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
                protected override ofElement Render()
                {
                    ++NestedRenders1;

                    var (count, setCount) = UseState(0);

                    UseEffect(() =>
                    {
                        if (count < 10)
                            setCount(count + 1);
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