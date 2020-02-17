using System;
using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class LiftingStateUpTests
    {
        public class Basic : ofComponent
        {
            public static int Renders;

            protected override ofElement Render()
            {
                var (count, setCount) = UseState(0);

                return new Nested(count, setCount);
            }

            class Nested : ofComponent
            {
                [Prop] readonly int _count;
                [Prop] readonly Action<int> _setCount;

                public Nested(int count, Action<int> setCount)
                {
                    _count    = count;
                    _setCount = setCount;
                }

                protected override ofElement Render()
                {
                    if (_count < 10)
                        _setCount(_count + 1);

                    ++Renders;

                    return null;
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(Renders, Is.EqualTo(11));

                node.RenderElement(this);

                Assert.That(Renders, Is.EqualTo(12));
            }
        }
    }
}