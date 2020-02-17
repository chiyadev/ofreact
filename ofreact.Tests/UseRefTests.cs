using System;
using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class UseRefTests
    {
        public class MismatchingType : ofComponent
        {
            public bool Alt;

            protected override ofElement Render()
            {
                if (Alt)
                    _ = UseRef<int>().Current;

                else
                    _ = UseRef<double>().Current;

                return null;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                node.RenderElement(this);

                Alt = true;

                Assert.That(() => node.RenderElement(this), Throws.InstanceOf<InvalidCastException>());
            }
        }
    }
}