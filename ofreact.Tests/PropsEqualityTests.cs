using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class PropsEqualityTests
    {
        class Element : ofElement
        {
            [Prop] public readonly string Prop1;
            [Prop] public readonly int Prop2;

            public Element(string prop1 = default, int prop2 = default, object key = default) : base(key)
            {
                Prop1 = prop1;
                Prop2 = prop2;
            }
        }

        [Test]
        public void AreEqual()
        {
            var key = new object();

            var element1 = new Element("test", 777, key);
            var element2 = new Element("test", 777, key);

            Assert.That(PropComparer.Equal(element1, element2), Is.True);
        }

        [Test]
        public void AreNotEqualString()
        {
            var key = new object();

            var element1 = new Element("test1", 777, key);
            var element2 = new Element("test2", 777, key);

            Assert.That(PropComparer.Equal(element1, element2), Is.False);
        }

        [Test]
        public void AreNotEqualInt()
        {
            var key = new object();

            var element1 = new Element("test", 777, key);
            var element2 = new Element("test", 778, key);

            Assert.That(PropComparer.Equal(element1, element2), Is.False);
        }

        [Test]
        public void AreNotEqualReference()
        {
            var key = new object();

            var element1 = new Element("test", 777, key);
            var element2 = new Element("test", 777, new object());

            Assert.That(PropComparer.Equal(element1, element2), Is.False);
        }

        class PrivateField : ofElement
        {
            [Prop] readonly string _prop1;

            public PrivateField(string prop1)
            {
                _prop1 = prop1;
            }
        }

        [Test]
        public void DoNotIgnorePrivateField() => Assert.That(PropComparer.Equal(new PrivateField("1"), new PrivateField("2")), Is.False);

        class PrivateFieldInherited1 : PrivateField
        {
            [Prop] readonly string _prop2 = "prop2";

            public PrivateFieldInherited1(string prop1) : base(prop1) { }
        }

        class PrivateFieldInherited2 : PrivateFieldInherited1
        {
            [Prop] readonly string _prop3 = "prop3";

            public PrivateFieldInherited2(string prop1) : base(prop1) { }
        }

        [Test]
        public void MultiLevelPrivateFields() => Assert.That(PropComparer.Equal(new PrivateFieldInherited2("1"), new PrivateFieldInherited2("1")), Is.True);

        [Test]
        public void MultiLevelPrivateFieldsNeq() => Assert.That(PropComparer.Equal(new PrivateFieldInherited2("1"), new PrivateFieldInherited2("2")), Is.False);

        [Test]
        public void ReferenceEqual()
        {
            var element = new PrivateField("");

            Assert.That(PropComparer.Equal(element, element), Is.True);
        }

        [Test]
        public void MismatchingTypes() => Assert.That(PropComparer.Equal(new PrivateField(""), new Element()), Is.False);
    }
}