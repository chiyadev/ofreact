using NUnit.Framework;

namespace ofreact.Tests
{
    public class PropEqualityComparerTests
    {
        class MyElement : ofElement
        {
            [Prop] public readonly string Prop1;
            [Prop] public readonly int Prop2;

            public MyElement(string prop1 = default, int prop2 = default, object key = default) : base(key)
            {
                Prop1 = prop1;
                Prop2 = prop2;
            }
        }

        [Test]
        public void AreEqual()
        {
            var key = new object();

            var element1 = new MyElement("test", 777, key);
            var element2 = new MyElement("test", 777, key);

            Assert.That(PropEqualityComparer.Equals(element1, element2), Is.True);
        }

        [Test]
        public void AreNotEqualString()
        {
            var key = new object();

            var element1 = new MyElement("test1", 777, key);
            var element2 = new MyElement("test2", 777, key);

            Assert.That(PropEqualityComparer.Equals(element1, element2), Is.False);
        }

        [Test]
        public void AreNotEqualInt()
        {
            var key = new object();

            var element1 = new MyElement("test", 777, key);
            var element2 = new MyElement("test", 778, key);

            Assert.That(PropEqualityComparer.Equals(element1, element2), Is.False);
        }

        [Test]
        public void AreNotEqualRef()
        {
            var key = new object();

            var element1 = new MyElement("test", 777, key);
            var element2 = new MyElement("test", 777, new object());

            Assert.That(PropEqualityComparer.Equals(element1, element2), Is.False);
        }

        class Private : ofElement
        {
            [Prop] readonly string _prop1;

            public Private(string prop1)
            {
                _prop1 = prop1;
            }
        }

        [Test]
        public void IgnorePrivateField() => Assert.That(PropEqualityComparer.Equals(new Private("1"), new Private("2")), Is.True);

        [Test]
        public void ReferenceEqual()
        {
            var element = new Private("");

            Assert.That(PropEqualityComparer.Equals(element, element), Is.True);
        }

        [Test]
        public void MismatchingTypes() => Assert.That(PropEqualityComparer.Equals(new Private(""), new MyElement()), Is.False);
    }
}