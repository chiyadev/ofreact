using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    public class BoundEffectTests
    {
        class Element1 : ofElement
        {
            public static int Effect;
            public static int Cleanup;

            [Effect]
            EffectCleanupDelegate Effect1()
            {
                ++Effect;

                return () => ++Cleanup;
            }
        }

        [Test]
        public void EveryRender()
        {
            var node = new ofRootNode();

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

        class Element2 : ofElement
        {
            public static int Dependency;
            public static int Effect;
            public static int Cleanup;

            public int Dep;

            public Element2()
            {
                Dep = Dependency;
            }

            [Effect(nameof(Dep))]
            EffectCleanupDelegate Effect2()
            {
                ++Effect;

                return () => ++Cleanup;
            }
        }

        [Test]
        public void DependencyChange()
        {
            var node = new ofRootNode();

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

        class Element3 : ofElement
        {
            public static int Effect;
            public static int Cleanup;

            [Effect(Once = true)]
            EffectCleanupDelegate Effect3()
            {
                ++Effect;

                return () => ++Cleanup;
            }
        }

        [Test]
        public void OnlyMount()
        {
            var node = new ofRootNode();

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

        class Element4 : ofElement
        {
            public static string StaticField;

            [Effect(nameof(StaticField))]
            void Effect() { }
        }

        [Test]
        public void NoStaticFieldDep()
        {
            var node = new ofRootNode();

            Assert.That(() => node.RenderElement(new Element4()), Throws.InstanceOf<ArgumentException>());
        }

        class Element5 : ofElement
        {
            string _privateField;

            [Effect(nameof(_privateField))]
            void Effect() { }
        }

        [Test]
        public void PrivateFieldDep()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element5());
        }

        class Element6 : ofComponent
        {
            static bool _rendered;

            // field gets evaluated before effect parameters, so value will be 100
            [State(100)] readonly StateObject<int> _value;

            [Effect]
            void Effect([State(50)] int value, [State("test")] string test)
            {
                Assert.That(_rendered, Is.True);

                Assert.That(value, Is.EqualTo(100));
                Assert.That(_value.Current, Is.EqualTo(100));

                Assert.That(test, Is.EqualTo("test"));
            }

            protected override ofElement Render()
            {
                Assert.That(_rendered, Is.False);

                Assert.That(_value.Current, Is.EqualTo(100));

                _rendered = true;

                return null;
            }
        }

        [Test]
        public void BindUnwrappedParameter()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element6());
        }

        class Element7 : ofComponent
        {
            [Ref] readonly RefObject<int> _count;

            [Effect]
            void Effect([Ref] RefObject<int> count) => Assert.That(count.Current, Is.EqualTo(7));

            protected override ofElement Render()
            {
                Assert.That(_count.Current, Is.EqualTo(0));

                _count.Current = 7;

                return null;
            }
        }

        [Test]
        public void BindParameter()
        {
            using var node = new ofRootNode();

            node.RenderElement(new Element7());
        }
    }
}