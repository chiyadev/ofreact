using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class RenderDiagnosticsTests
    {
        public class RenderedList : ofComponent
        {
            protected override ofElement Render() => new ofFragment
            {
                new Nested1(),
                new Nested2()
            };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new Nested3();

                public class Nested3 : ofElement { }
            }

            class Nested2 : ofElement { }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                node.Diagnostics.EnsureRendered(
                    typeof(RenderedList),
                    typeof(ofFragment),
                    typeof(Nested1),
                    typeof(Nested1.Nested3),
                    typeof(Nested2));
            }
        }

        public class RenderedListNegative : ofComponent
        {
            protected override ofElement Render() => new ofFragment
            {
                new Nested1(),
                new Nested2()
            };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new Nested3();

                class Nested3 : ofElement { }
            }

            class Nested2 : ofElement { }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(() => node.Diagnostics.EnsureRendered(
                    typeof(RenderedList),
                    typeof(ofFragment),
                    typeof(Nested2)), Throws.InstanceOf<ArgumentException>());
            }
        }
    }
}