using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class FragmentTests
    {
        public class RenderAll : ofComponent
        {
            protected override ofElement Render()
            {
                ++UseContext<RenderCountContext>().Count;

                return null;
            }

            class RenderCountContext
            {
                public int Count;
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                var context = new RenderCountContext();

                // ofContext is ofFragment
                node.RenderElement(new ofContext<RenderCountContext>(value: context)
                {
                    this,
                    this,
                    this,
                    this,
                    this
                });

                Assert.That(context.Count, Is.EqualTo(5));
            }
        }

        public class MountCounting : ofComponent
        {
            public static int Mount;
            public static int Unmount;

            protected override ofElement Render()
            {
                UseEffect(() =>
                {
                    ++Mount;

                    return () => ++Unmount;
                }, null);

                return null;
            }

            public MountCounting(ElementKey key = default) : base(key) { }
        }

        [Test]
        public void Complex()
        {
            var node = new ofRootNode();

            Assert.That(MountCounting.Mount, Is.EqualTo(0));
            Assert.That(MountCounting.Unmount, Is.EqualTo(0));

            node.RenderElement(new[]
            {
                new MountCounting("first"),
                new MountCounting("second"),
                new MountCounting("third")
            });

            Assert.That(MountCounting.Mount, Is.EqualTo(3));
            Assert.That(MountCounting.Unmount, Is.EqualTo(0));

            node.RenderElement(new[]
            {
                new MountCounting("first"),
                new MountCounting("third")
            });

            Assert.That(MountCounting.Mount, Is.EqualTo(3));
            Assert.That(MountCounting.Unmount, Is.EqualTo(1));

            node.RenderElement(new[]
            {
                new MountCounting("third"),
                new MountCounting("first")
            });

            Assert.That(MountCounting.Mount, Is.EqualTo(3));
            Assert.That(MountCounting.Unmount, Is.EqualTo(1));

            node.RenderElement(new[]
            {
                new MountCounting("third"),
                new MountCounting("second"),
                new MountCounting("first")
            });

            Assert.That(MountCounting.Mount, Is.EqualTo(4));
            Assert.That(MountCounting.Unmount, Is.EqualTo(1));

            node.RenderElement(new[]
            {
                new MountCounting("first")
            });

            Assert.That(MountCounting.Mount, Is.EqualTo(4));
            Assert.That(MountCounting.Unmount, Is.EqualTo(3));

            node.Dispose();

            Assert.That(MountCounting.Mount, Is.EqualTo(4));
            Assert.That(MountCounting.Unmount, Is.EqualTo(4));
        }

        [Test]
        public void IgnoreNullChild()
        {
            using var node = new ofRootNode();

            node.RenderElement(new[]
            {
                new ofFragment(),
                null,
                new ofFragment()
            });
        }
    }
}