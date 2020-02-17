using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class BoundRefTests
    {
        public class Ref : ofComponent
        {
            public int CurrentCount;
            public string CurrentString;

            [Ref] public readonly RefObject<int> Count;
            [Ref] public readonly RefObject<string> String;

            protected override ofElement Render()
            {
                CurrentCount  = ++Count.Current;
                CurrentString = String.Current = Count.Current.ToString();

                return null;
            }

            [Test]
            public void Test()
            {
                Assert.That(CurrentCount, Is.EqualTo(0));
                Assert.That(CurrentString, Is.Null);

                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(CurrentCount, Is.EqualTo(1));
                Assert.That(CurrentString, Is.EqualTo("1"));

                node.RenderElement(this);

                Assert.That(CurrentCount, Is.EqualTo(2));
                Assert.That(CurrentString, Is.EqualTo("2"));
            }
        }

        public class InitialValue : ofComponent
        {
            public bool Rendered;

            [Ref(777)] public readonly RefObject<int> Number;

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
    }
}