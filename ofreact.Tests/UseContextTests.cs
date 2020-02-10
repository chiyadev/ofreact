using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    public class UseContextTests
    {
        class Context
        {
            public string Value;
        }

        class Element1 : ofComponent
        {
            public static bool ContextFound;

            protected override ofElement Render() =>
                new ofContext<Context>(value: new Context { Value = "test" })
                {
                    new Nested1()
                };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new Nested2();

                class Nested2 : ofComponent
                {
                    protected override ofElement Render() => new Nested3();

                    class Nested3 : ofComponent
                    {
                        protected override ofElement Render()
                        {
                            var context = UseContext<Context>();

                            Assert.That(context, Is.Not.Null);
                            Assert.That(context.Value, Is.EqualTo("test"));

                            ContextFound = true;

                            return null;
                        }
                    }
                }
            }
        }

        [Test]
        public void MultiLevelNestedContext()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element1());

            Assert.That(Element1.ContextFound, Is.True);
        }

        class Element2 : ofComponent
        {
            public static bool ContextFound;

            protected override ofElement Render() =>
                new ofContext<Context>(value: new Context { Value = "test1" })
                {
                    new Nested1()
                };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() =>
                    new ofContext<Context>(value: new Context { Value = "test2" })
                    {
                        new Nested2()
                    };

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Value, Is.EqualTo("test2"));

                        ContextFound = true;

                        return null;
                    }
                }
            }
        }

        [Test]
        public void NearestContext()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element2());

            Assert.That(Element2.ContextFound, Is.True);
        }

        class Element3 : ofComponent
        {
            public static bool ContextNotFound;

            protected override ofElement Render()
            {
                var context = UseContext<Context>();

                Assert.That(context, Is.Null);

                ContextNotFound = true;

                return null;
            }
        }

        [Test]
        public void NonexistentContext()
        {
            using var node = new ofNodeRoot();

            node.RenderElement(new Element3());

            Assert.That(Element3.ContextNotFound, Is.True);
        }

        class DisposableContext : IDisposable
        {
            public static bool Disposed;

            public void Dispose() => Disposed = true;
        }

        class Element4 : ofComponent
        {
            protected override ofElement Render() => new ofContext<DisposableContext>(value: new DisposableContext())
            {
                new Nested()
            };

            class Nested : ofComponent
            {
                protected override ofElement Render()
                {
                    var context = UseContext<DisposableContext>();

                    Assert.That(context, Is.Not.Null);

                    return null;
                }
            }
        }

        [Test]
        public void DisposeContext()
        {
            using var node = new ofNodeRoot();

            Assert.That(DisposableContext.Disposed, Is.False);

            node.RenderElement(new Element4());

            Assert.That(DisposableContext.Disposed, Is.True);
        }
    }
}