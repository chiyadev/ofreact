using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class UseEffectTests
    {
        public class EveryRender : ofComponent
        {
            public int Effect;
            public int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                });

                return null;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                Assert.That(Effect, Is.EqualTo(0));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(2));
                Assert.That(Cleanup, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(2));

                node.Dispose();

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(3));
            }
        }

        public class DependencyChange : ofComponent
        {
            public int Dependency;
            public int Effect;
            public int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                }, Dependency);

                return null;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                Assert.That(Effect, Is.EqualTo(0));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                ++Dependency;

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(2));
                Assert.That(Cleanup, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(2));
                Assert.That(Cleanup, Is.EqualTo(1));

                ++Dependency;

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(2));

                node.Dispose();

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(3));
            }
        }

        public class OnlyMount : ofComponent
        {
            public int Effect;
            public int Cleanup;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Effect;

                    return () => ++Cleanup;
                }, null);

                return null;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                Assert.That(Effect, Is.EqualTo(0));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.Dispose();

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(1));
            }
        }

        public class ChangeStateInCleanup : ofComponent
        {
            protected override ofElement Render()
            {
                var obj = UseRef("");

                UseEffect(() => () => obj.Current = "success", null);

                return null;
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);
            }
        }
    }
}