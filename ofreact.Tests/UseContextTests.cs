using System;
using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    public class UseContextTests
    {
        class Element1 : ofComponent
        {
            class Context
            {
                public string Value;
            }

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
            using var node = new ofRootNode();

            node.RenderElement(new Element1());

            Assert.That(Element1.ContextFound, Is.True);
        }

        class Element2 : ofComponent
        {
            class Context
            {
                public string Value;
            }

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
            using var node = new ofRootNode();

            node.RenderElement(new Element2());

            Assert.That(Element2.ContextFound, Is.True);
        }

        class Element3 : ofComponent
        {
            class Context
            {
                public string Value;
            }

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
            using var node = new ofRootNode();

            node.RenderElement(new Element3());

            Assert.That(Element3.ContextNotFound, Is.True);
        }

        class Element4 : ofComponent
        {
            public class Context : IDisposable
            {
                public static bool Disposed;

                public void Dispose() => Disposed = true;
            }

            protected override ofElement Render() => new ofContext<Context>(value: new Context())
            {
                new Nested()
            };

            class Nested : ofComponent
            {
                protected override ofElement Render()
                {
                    var context = UseContext<Context>();

                    Assert.That(context, Is.Not.Null);

                    return null;
                }
            }
        }

        [Test]
        public void DisposeContext()
        {
            var node = new ofRootNode();

            Assert.That(Element4.Context.Disposed, Is.False);

            node.RenderElement(new Element4());

            Assert.That(Element4.Context.Disposed, Is.False);

            node.Dispose();

            Assert.That(Element4.Context.Disposed, Is.True);
        }

        [Test]
        public void DisposeContextOnChange() { }

        class Element5 : ofComponent
        {
            public static int Rendered;
            public static StateObject<int> State;

            protected override ofElement Render() => new Nested1();

            class Context
            {
                [Prop] public string Value;
            }

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new ofContext<Context>(value: new Context { Value = "test" })
                {
                    new Nested2()
                };

                class Nested2 : ofComponent
                {
                    [State] readonly StateObject<int> _state;

                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Value, Is.EqualTo("test"));

                        ++Rendered;
                        State = _state;

                        return null;
                    }
                }
            }
        }

        [Test]
        public void PartialUpdateContextAvailable()
        {
            using var node = new ofRootNode();

            Assert.That(Element5.Rendered, Is.EqualTo(0));

            node.RenderElement(new Element5());

            Assert.That(Element5.Rendered, Is.EqualTo(1));

            node.RenderElement(new Element5());

            Assert.That(Element5.Rendered, Is.EqualTo(1));

            Element5.State.Current++;

            node.RenderElement(new Element5());

            Assert.That(Element5.Rendered, Is.EqualTo(2));
        }

        class Element6 : ofComponent
        {
            public static int Nested1Rendered;
            public static int Nested2Rendered;

            public static bool RemoveNested2;

            protected override ofElement Render() => new ofContext<Context>(value: new Context
            {
                Count = Nested2Rendered
            })
            {
                new Nested1(RemoveNested2)
            };

            class Context
            {
                [Prop] public int Count;
            }

            class Nested1 : ofComponent
            {
                [Prop] readonly bool _value;

                public Nested1(bool value)
                {
                    _value = value;
                }

                protected override ofElement Render()
                {
                    ++Nested1Rendered;

                    if (_value)
                        return null;

                    return new Nested2();
                }

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Count, Is.EqualTo(Nested2Rendered));

                        ++Nested2Rendered;

                        return null;
                    }
                }
            }
        }

        [Test]
        public void ContextChangeRerender()
        {
            using var node = new ofRootNode();

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(0));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(0));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(1));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(2));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(3));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(4));

            Element6.RemoveNested2 = true;

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(2));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(4));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(2));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(4));

            Element6.RemoveNested2 = false;

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(3));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(5));

            node.RenderElement(new Element6());

            Assert.That(Element6.Nested1Rendered, Is.EqualTo(3));
            Assert.That(Element6.Nested2Rendered, Is.EqualTo(6));
        }

        class Element7 : ofComponent
        {
            public static int Nested1Rendered;
            public static int Nested2Rendered;

            protected override ofElement Render() => new ofContext<Context>(value: new Context
            {
                IgnoreMe = $"test {Nested1Rendered}"
            })
            {
                new Nested1()
            };

            class Context
            {
                public string IgnoreMe;
            }

            class Nested1 : ofComponent
            {
                protected override ofElement Render()
                {
                    ++Nested1Rendered;

                    return new Nested2();
                }

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);

                        ++Nested2Rendered;

                        return null;
                    }
                }
            }
        }

        [Test]
        public void EmptyContext()
        {
            using var node = new ofRootNode();

            Assert.That(Element7.Nested1Rendered, Is.EqualTo(0));
            Assert.That(Element7.Nested2Rendered, Is.EqualTo(0));

            node.RenderElement(new Element7());

            Assert.That(Element7.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element7.Nested2Rendered, Is.EqualTo(1));

            node.RenderElement(new Element7());

            Assert.That(Element7.Nested1Rendered, Is.EqualTo(1));
            Assert.That(Element7.Nested2Rendered, Is.EqualTo(1));
        }
    }
}