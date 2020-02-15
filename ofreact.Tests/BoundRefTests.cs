using NUnit.Framework;

namespace ofreact.Tests
{
    public class BoundRefTests
    {
        class Element1 : ofComponent
        {
            public static int CurrentCount;
            public static string CurrentString;

            [Ref] public readonly RefObject<int> Count;
            [Ref] public readonly RefObject<string> String;

            protected override ofElement Render()
            {
                CurrentCount  = ++Count.Current;
                CurrentString = String.Current = Count.Current.ToString();

                return null;
            }
        }

        [Test]
        public void Ref()
        {
            Assert.That(Element1.CurrentCount, Is.EqualTo(0));
            Assert.That(Element1.CurrentString, Is.Null);

            using var node = new ofRootNode();

            node.RenderElement(new Element1());

            Assert.That(Element1.CurrentCount, Is.EqualTo(1));
            Assert.That(Element1.CurrentString, Is.EqualTo("1"));

            node.RenderElement(new Element1());

            Assert.That(Element1.CurrentCount, Is.EqualTo(2));
            Assert.That(Element1.CurrentString, Is.EqualTo("2"));
        }

        class Element2 : ofComponent
        {
            public static bool Rendered;

            [Ref(777)] public readonly RefObject<int> Number;

            protected override ofElement Render()
            {
                Assert.That(Number.Current, Is.EqualTo(777));

                Rendered = true;

                return null;
            }
        }

        [Test]
        public void InitialValue()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element2());

            Assert.That(Element2.Rendered, Is.True);
        }
    }
}