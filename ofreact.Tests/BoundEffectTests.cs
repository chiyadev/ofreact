using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    [TestFixture]
    public class BoundEffectTests
    {
        public class EveryRender : ofElement
        {
            public int Effect;
            public int Cleanup;

            [Effect]
            EffectCleanupDelegate Effect1()
            {
                ++Effect;

                return () => ++Cleanup;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

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

        public class DependencyChange : ofElement
        {
            public int Dependency;
            public int Effect;
            public int Cleanup;

            [Effect(nameof(Dependency))]
            EffectCleanupDelegate Effect2()
            {
                ++Effect;

                return () => ++Cleanup;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

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

        public class OnlyMount : ofElement
        {
            public int Effect;
            public int Cleanup;

            [Effect(Once = true)]
            EffectCleanupDelegate EffectMethod()
            {
                ++Effect;

                return () => ++Cleanup;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

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

        public class DisallowStaticFieldDependency : ofElement
        {
            public static string Field;

            [Effect(nameof(Field))]
            void Effect() { }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                Assert.That(() => node.RenderElement(this), Throws.InstanceOf<ArgumentException>());
            }
        }

        public class PrivateFieldDependency : ofElement
        {
            int _dependency;

            public int Effect;
            public int Cleanup;

            [Effect(nameof(_dependency))]
            EffectCleanupDelegate Effect2()
            {
                ++Effect;

                return () => ++Cleanup;
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(1));
                Assert.That(Cleanup, Is.EqualTo(0));

                ++_dependency;

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(2));
                Assert.That(Cleanup, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(2));
                Assert.That(Cleanup, Is.EqualTo(1));

                ++_dependency;

                node.RenderElement(this);

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(2));

                node.Dispose();

                Assert.That(Effect, Is.EqualTo(3));
                Assert.That(Cleanup, Is.EqualTo(3));
            }
        }

        public class BindUnwrappedParameter : ofComponent
        {
            bool _rendered;

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

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);
            }
        }

        public class BindParameter : ofComponent
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

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);
            }
        }
    }
}