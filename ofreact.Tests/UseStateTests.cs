using NUnit.Framework;

namespace ofreact.Tests
{
    public class UseStateTests
    {
        class Element1 : ofComponent
        {
            public static int RenderCount;

            protected override ofElement Render()
            {
                var (count, setCount) = UseState(++RenderCount);

                UseEffect(() =>
                {
                    if (count < 10)
                        setCount(count + 1);
                });

                return null;
            }
        }

        [Test]
        public void StateUpdateTriggersRerender()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element1());

            Assert.That(Element1.RenderCount, Is.EqualTo(10));
        }

        class Element2 : ofComponent
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
                protected override ofElement Render()
                {
                    var (count, setCount) = UseState(++NestedRenders1);

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
        }

        [Test]
        public void StateUpdateTriggersPartialTreeRerender()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element2());

            Assert.That(Element2.ParentRenders, Is.EqualTo(1));
            Assert.That(Element2.NestedRenders1, Is.EqualTo(10));
            Assert.That(Element2.NestedRenders2, Is.EqualTo(1));
        }
    }
}