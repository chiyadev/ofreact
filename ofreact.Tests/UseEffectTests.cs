using NUnit.Framework;

namespace ofreact.Tests
{
    public class UseEffectTests
    {
        class Element1 : ofComponent
        {
            public static int Effect;
            public static int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                });

                return null;
            }
        }

        [Test]
        public void EveryRender()
        {
            var node = new ofNodeRoot();

            Assert.That(Element1.Effect, Is.EqualTo(0));
            Assert.That(Element1.Cleanup, Is.EqualTo(0));

            node.RenderElement(new Element1());

            Assert.That(Element1.Effect, Is.EqualTo(1));
            Assert.That(Element1.Cleanup, Is.EqualTo(0));

            node.Invalidate();
            node.RenderElement(new Element1());

            Assert.That(Element1.Effect, Is.EqualTo(2));
            Assert.That(Element1.Cleanup, Is.EqualTo(1));

            node.Invalidate();
            node.RenderElement(new Element1());

            Assert.That(Element1.Effect, Is.EqualTo(3));
            Assert.That(Element1.Cleanup, Is.EqualTo(2));

            node.Dispose();

            Assert.That(Element1.Effect, Is.EqualTo(3));
            Assert.That(Element1.Cleanup, Is.EqualTo(3));
        }

        class Element2 : ofComponent
        {
            public static int Dependency;
            public static int Effect;
            public static int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                }, Dependency);

                return null;
            }
        }

        [Test]
        public void DependencyChange()
        {
            var node = new ofNodeRoot();

            Assert.That(Element2.Effect, Is.EqualTo(0));
            Assert.That(Element2.Cleanup, Is.EqualTo(0));

            node.RenderElement(new Element2());

            Assert.That(Element2.Effect, Is.EqualTo(1));
            Assert.That(Element2.Cleanup, Is.EqualTo(0));

            node.Invalidate();
            node.RenderElement(new Element2());

            Assert.That(Element2.Effect, Is.EqualTo(1));
            Assert.That(Element2.Cleanup, Is.EqualTo(0));

            ++Element2.Dependency;

            node.Invalidate();
            node.RenderElement(new Element2());

            Assert.That(Element2.Effect, Is.EqualTo(2));
            Assert.That(Element2.Cleanup, Is.EqualTo(1));

            node.Invalidate();
            node.RenderElement(new Element2());

            Assert.That(Element2.Effect, Is.EqualTo(2));
            Assert.That(Element2.Cleanup, Is.EqualTo(1));

            ++Element2.Dependency;

            node.Invalidate();
            node.RenderElement(new Element2());

            Assert.That(Element2.Effect, Is.EqualTo(3));
            Assert.That(Element2.Cleanup, Is.EqualTo(2));

            node.Dispose();

            Assert.That(Element2.Effect, Is.EqualTo(3));
            Assert.That(Element2.Cleanup, Is.EqualTo(3));
        }

        class Element3 : ofComponent
        {
            public static int Effect;
            public static int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                }, null);

                return null;
            }
        }

        [Test]
        public void OnlyMount()
        {
            var node = new ofNodeRoot();

            Assert.That(Element3.Effect, Is.EqualTo(0));
            Assert.That(Element3.Cleanup, Is.EqualTo(0));

            node.RenderElement(new Element3());

            Assert.That(Element3.Effect, Is.EqualTo(1));
            Assert.That(Element3.Cleanup, Is.EqualTo(0));

            node.Invalidate();
            node.RenderElement(new Element3());

            Assert.That(Element3.Effect, Is.EqualTo(1));
            Assert.That(Element3.Cleanup, Is.EqualTo(0));

            node.Invalidate();
            node.RenderElement(new Element3());

            Assert.That(Element3.Effect, Is.EqualTo(1));
            Assert.That(Element3.Cleanup, Is.EqualTo(0));

            node.Dispose();

            Assert.That(Element3.Effect, Is.EqualTo(1));
            Assert.That(Element3.Cleanup, Is.EqualTo(1));
        }

        class Element4 : ofComponent
        {
            protected override ofElement Render()
            {
                var obj = UseRef("");

                UseEffect(() => () => obj.Current = "success", null);

                return null;
            }
        }

        [Test]
        public void ChangeStateInCleanup()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element4());
        }
    }
}